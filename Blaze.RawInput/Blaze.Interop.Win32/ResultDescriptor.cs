// Copyright © 2020 Infinisis

using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Blaze.Interop.Win32
{
    /// <summary>
    ///   Represents a descriptor used to provide a detailed message for a particular <see cref="Win32.Result"/>.
    /// </summary>
    public sealed class ResultDescriptor
    {
        /// <summary>
        ///   Gets the result.
        /// </summary>
        public Result Result { get; private set; }

        /// <summary>
        ///   Gets the HRESULT error code.
        /// </summary>
        /// <value>The HRESULT error code.</value>
        public int Code => Result.Code;

        /// <summary>
        ///   Gets the module name.
        /// </summary>
        public string Module { get; private set; }

        /// <summary>
        ///   Gets the native API-specific code.
        /// </summary>
        public string NativeApiCode { get; private set; }

        /// <summary>
        ///   Gets the API-specific code.
        /// </summary>
        public string ApiCode { get; private set; }

        /// <summary>
        ///   Gets a description of the result code.
        /// </summary>
        /// <value>A description of the result code; or <c>null</c> if there is no description available.</value>
        public string Description { get; set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="ResultDescriptor"/> class.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="module">The module name (ex: <c>Blaze.RawInput</c>).</param>
        /// <param name="apiCode">The API-specific error code (ex: <c>D2D1_ERR_...</c>).</param>
        /// <param name="description">The optional description of the result code. This can be <c>null</c>.</param>
        public ResultDescriptor(Result code, string module, string nativeApiCode, string apiCode, string description = null)
        {
            Result = code;
            Module = module;
            NativeApiCode = nativeApiCode;
            ApiCode = apiCode;
            Description = description ?? GetDescriptionFromResultCode(Code) ?? "Unknown";

            Descriptors.TryAdd(code, this);
        }


        /// <summary>
        ///   Determines whether the specified <see cref="ResultDescriptor"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="ResultDescriptor"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="ResultDescriptor"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ResultDescriptor other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return other.Result.Equals(Result);
        }

        /// <summary>
        ///   Determines   whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is ResultDescriptor descriptor)
                return Equals(descriptor);

            return false;
        }

        /// <summary>
        ///   Returns the hash code of this instance.
        /// </summary>
        /// <returns>The hash code of this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() => Result.GetHashCode();

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"HRESULT: [0x{Result.Code:X}], Module: [{Module}], ApiCode: [{NativeApiCode}/{ApiCode}], Message: {Description}";
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="ResultDescriptor"/> to <see cref="Win32.Result"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Result(ResultDescriptor result) => result.Result;

        /// <summary>
        ///   Performs an implicit conversion from <see cref="ResultDescriptor"/> to <see cref="int"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator int(ResultDescriptor result) => result.Result.Code;

        /// <summary>
        /// Performs an implicit conversion from <see cref="ResultDescriptor"/> to <see cref="uint"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator uint(ResultDescriptor result) => unchecked((uint) result.Result.Code);

        public static bool operator ==(ResultDescriptor left, Result right)
        {
            if (left is null)
                return false;

            return left.Result.Code == right.Code;
        }

        public static bool operator !=(ResultDescriptor left, Result right)
        {
            if (left is null)
                return false;

            return left.Result.Code != right.Code;
        }

        #region Registered result descriptors

        private static readonly ConcurrentDictionary<Result, ResultDescriptor> Descriptors = new ConcurrentDictionary<Result, ResultDescriptor>();


        /// <summary>
        ///   Finds the specified result descriptor.
        /// </summary>
        /// <param name="result">The result code.</param>
        /// <returns>A descriptor for the specified result.</returns>
        public static ResultDescriptor Find(Result result)
        {
            if (!Descriptors.TryGetValue(result, out ResultDescriptor descriptor))
            {
                descriptor = new ResultDescriptor(result, "Unknown", "Unknown", "Unknown");
            }

            return descriptor;
        }

        #endregion

        #region Utility methods

        private static string GetDescriptionFromResultCode(int resultCode)
        {
            const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
            const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
            const int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

            const int FORMAT_FLAGS = FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS;

            IntPtr buffer = IntPtr.Zero;
            FormatMessageW(FORMAT_FLAGS, IntPtr.Zero, resultCode, 0, ref buffer, 0, IntPtr.Zero);
            var description = Marshal.PtrToStringUni(buffer);
            Marshal.FreeHGlobal(buffer);

            return description;
        }

        [DllImport("kernel32.dll", EntryPoint = "FormatMessageW")]
        private static extern uint FormatMessageW(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, ref IntPtr lpBuffer, int nSize, IntPtr Arguments);

        #endregion
    }
}