// Copyright © 2020 Infinisis

using System;
using System.Runtime.InteropServices;

namespace Blaze.Interop.Win32
{
    /// <summary>
    ///   Represents the result of calling a Win32 API or COM method.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Result : IEquatable<Result>
    {
        /// <summary>
        ///   Gets the result code.
        /// </summary>
        /// <value>The result code (HRESULT).</value>
        public int Code { get; }

        /// <summary>
        ///   Gets a value indicating whether this <see cref="Result"/> represents a success.
        /// </summary>
        /// <value><c>true</c> if the result is a success; otherwise, <c>false</c>.</value>
        public bool Success => Code >= 0;

        /// <summary>
        ///   Gets a value indicating whether this <see cref="Result"/> represents a failure.
        /// </summary>
        /// <value><c>true</c> if the result is a failure; otherwise, <c>false</c>.</value>
        public bool Failure => Code < 0;


        /// <summary>
        ///   Initializes a new instance of the <see cref="Result"/> struct.
        /// </summary>
        /// <param name="code">The HRESULT error code.</param>
        public Result(int code)
        {
            Code = code;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Result"/> struct.
        /// </summary>
        /// <param name="code">The HRESULT error code.</param>
        public Result(uint code)
        {
            Code = unchecked((int) code);
        }


        /// <summary>
        ///   Performs an explicit conversion from <see cref="Result"/> to <see cref="int"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator int(Result result) => result.Code;

        /// <summary>
        ///   Performs an explicit conversion from <see cref="Result"/> to <see cref="uint"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator uint(Result result) => unchecked((uint) result.Code);

        /// <summary>
        ///   Performs an implicit conversion from <see cref="int"/> to <see cref="Result"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Result(int result) => new Result(result);

        /// <summary>
        ///   Performs an implicit conversion from <see cref="uint"/> to <see cref="Result"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Result(uint result) => new Result(result);


        /// <summary>
        ///   Determines whether the specified <see cref="Result"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The other result to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="Result"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Result other) => (Code == other.Code);

        /// <summary>
        ///   Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Result result)
                return Equals(result);

            return false;
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode() => Code;

        public static bool operator ==(Result left, Result right) => left.Code == right.Code;

        public static bool operator !=(Result left, Result right) => left.Code != right.Code;

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString() => $"HRESULT = 0x{Code:X}";


        /// <summary>
        ///   Gets a <see cref="Result"/> from an <see cref="Exception"/>.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>The associated result code.</returns>
        public static Result FromException(Exception ex) => new Result(Marshal.GetHRForException(ex));

        /// <summary>
        ///   Gets a <see cref="Result"/> from a Win32 error code.
        /// </summary>
        /// <param name="errorCode">The Win32 error code.</param>
        /// <returns>The associated result code.</returns>
        public static Result FromWin32Error(int errorCode)
        {
            const int FACILITY_WIN32 = 7;

            return errorCode <= 0
                ? errorCode
                : (int)((errorCode & 0x0000FFFF) | (FACILITY_WIN32 << 16) | 0x80000000);
        }

        #region Known result codes

        /// <summary>
        ///   Represents a result code of a successful operation.
        /// </summary>
        /// <unmanaged>S_OK</unmanaged>
        public static Result Ok = new Result(unchecked(0x00000000));

        /// <summary>
        ///   Represents a result code indicating the boolean value <c>false</c>.
        /// </summary>
        public static Result False = new Result(unchecked(0x00000001));

        /// <summary>
        ///   Represents a result code of an aborted operation.
        /// </summary>
        /// <unmanaged>E_ABORT</unmanaged>
        public static Result Abort = new Result(unchecked(0x80004004u));

        /// <summary>
        ///   Represents a result code of a general access denied error.
        /// </summary>
        /// <unmanaged>E_ACCESSDENIED</unmanaged>
        public static Result AccessDenied = new Result(unchecked(0x80070005u));

        /// <summary>
        ///   Represents a result code of an unspecified failure.
        /// </summary>
        /// <unmanaged>E_FAIL</unmanaged>
        public static Result Fail = new Result(unchecked(0x80004005u));

        /// <summary>
        ///   Represents a result code of an invalid handle.
        /// </summary>
        /// <unmanaged>E_HANDLE</unmanaged>
        public static Result InvalidHandle = new Result(unchecked(0x80070006u));

        /// <summary>
        ///   Represents a result code of an invalid argument.
        /// </summary>
        /// <unmanaged>E_INVALIDARG</unmanaged>
        public static Result InvalidArgument = new Result(unchecked(0x80070057u));

        /// <summary>
        ///   Represents a result code of an unsupported interface.
        /// </summary>
        /// <unmanaged>E_NOINTERFACE</unmanaged>
        public static Result InterfaceNotSupported = new Result(unchecked(0x80004002u));

        /// <summary>
        ///   Represents a result code of a not implemented operation.
        /// </summary>
        /// <unmanaged>E_NOTIMPL</unmanaged>
        public static Result NotImplemented = new Result(unchecked(0x80004001u));

        /// <summary>
        ///   Represents a result code of a failure to allocate necessary memory.
        /// </summary>
        /// <unmanaged>E_OUTOFMEMORY</unmanaged>
        public static Result OutOfMemory = new Result(unchecked(0x8007000Eu));

        /// <summary>
        ///   Represents a result code of a pointer that is not valid.
        /// </summary>
        /// <unmanaged>E_POINTER</unmanaged>
        public static Result InvalidPointer = new Result(unchecked(0x80004003u));

        /// <summary>
        ///   Represents a result code of an unexpected failure.
        /// </summary>
        /// <unmanaged>E_UNEXPECTED</unmanaged>
        public static Result UnexpectedFailure = new Result(unchecked(0x8000FFFFu));

        #endregion
    }
}
