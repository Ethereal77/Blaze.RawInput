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
        public readonly int Code { get; }

        /// <summary>
        ///   Gets a value indicating whether this <see cref="Result"/> represents a success.
        /// </summary>
        /// <value><c>true</c> if the result is a success; otherwise, <c>false</c>.</value>
        public readonly bool Success => Code >= 0;

        /// <summary>
        ///   Gets a value indicating whether this <see cref="Result"/> represents a failure.
        /// </summary>
        /// <value><c>true</c> if the result is a failure; otherwise, <c>false</c>.</value>
        public readonly bool Failure => Code < 0;


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
        public readonly bool Equals(Result other) => (Code == other.Code);

        /// <summary>
        ///   Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public readonly override bool Equals(object obj)
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
        public readonly override int GetHashCode() => Code;

        public static bool operator ==(Result left, Result right) => left.Code == right.Code;

        public static bool operator !=(Result left, Result right) => left.Code != right.Code;

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public readonly override string ToString() => $"Result = 0x{Code:X}";


        /// <summary>
        ///   Gets a <see cref="Result"/> from an <see cref="Exception"/>.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>The associated result code.</returns>
        public static Result FromException(Exception ex) => new Result(ex.HResult);

        /// <summary>
        ///   Gets a <see cref="Result"/> from a Win32 error code.
        /// </summary>
        /// <param name="errorCode">The Win32 error code. This code can be obtained from <see cref="Marshal.GetLastWin32Error"/></param>
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
        public static readonly Result Ok = new Result(0x00000000u);

        /// <summary>
        ///   Represents a result code indicating the boolean value <c>false</c>.
        /// </summary>
        public static readonly Result False = new Result(0x00000001u);

        /// <summary>
        ///   Represents a result code indicating an aborted operation.
        /// </summary>
        /// <unmanaged>E_ABORT</unmanaged>
        public static readonly ResultDescriptor Abort = new ResultDescriptor(0x80004004u, "General", "E_ABORT", "Operation aborted");

        /// <summary>
        ///   Represents a result code indicating a general access denied error.
        /// </summary>
        /// <unmanaged>E_ACCESSDENIED</unmanaged>
        public static readonly ResultDescriptor AccessDenied = new ResultDescriptor(0x80070005u, "General", "E_ACCESSDENIED", "General access denied error");

        /// <summary>
        ///   Represents a result code indicating an unspecified failure.
        /// </summary>
        /// <unmanaged>E_FAIL</unmanaged>
        public static readonly ResultDescriptor Fail = new ResultDescriptor(0x80004005u, "General", "E_FAIL", "Unspecified error");

        /// <summary>
        ///   Represents a result code indicating an invalid handle.
        /// </summary>
        /// <unmanaged>E_HANDLE</unmanaged>
        public static readonly ResultDescriptor InvalidHandle = new ResultDescriptor(0x80070006u, "General", "E_HANDLE", "Invalid handle");

        /// <summary>
        ///   Represents a result code indicating an invalid argument.
        /// </summary>
        /// <unmanaged>E_INVALIDARG</unmanaged>
        public static readonly ResultDescriptor InvalidArgument = new ResultDescriptor(0x80070057u, "General", "E_INVALIDARG", "Invalid arguments");

        /// <summary>
        ///   Represents a result code indicating an unsupported interface.
        /// </summary>
        /// <unmanaged>E_NOINTERFACE</unmanaged>
        public static readonly ResultDescriptor InterfaceNotSupported = new ResultDescriptor(0x80004002u, "General", "E_NOINTERFACE", "No such interface supported");

        /// <summary>
        ///   Represents a result code of a not implemented operation.
        /// </summary>
        /// <unmanaged>E_NOTIMPL</unmanaged>
        public static readonly ResultDescriptor NotImplemented = new ResultDescriptor(0x80004001u, "General", "E_NOTIMPL", "Not implemented");

        /// <summary>
        ///   Represents a result code indicating a failure to allocate necessary memory.
        /// </summary>
        /// <unmanaged>E_OUTOFMEMORY</unmanaged>
        public static readonly ResultDescriptor OutOfMemory = new ResultDescriptor(0x8007000Eu, "General", "E_OUTOFMEMORY", "Out of memory");

        /// <summary>
        ///   Represents a result code indicating that a pointer is not valid.
        /// </summary>
        /// <unmanaged>E_POINTER</unmanaged>
        public static readonly ResultDescriptor InvalidPointer = new ResultDescriptor(0x80004003u, "General", "E_POINTER", "Invalid pointer");

        /// <summary>
        ///   Represents a result code indicating an unexpected failure.
        /// </summary>
        /// <unmanaged>E_UNEXPECTED</unmanaged>
        public static readonly ResultDescriptor UnexpectedFailure = new ResultDescriptor(0x8000FFFFu, "General", "E_UNEXPECTED", "Catastrophic failure");

        /// <summary>
        ///   Represents a result code indicating that the data necessary to complete an operation is not yet available.
        /// </summary>
        /// <unmanaged>E_PENDING</unmanaged>
        public static readonly ResultDescriptor Pending = new ResultDescriptor(0x8000000Au, "General", "E_PENDING", "Data not yet available");

        /// <summary>
        ///   Represents a result code indicating that a wait operation on a resource was abandonned.
        /// </summary>
        /// <unmanaged>WAIT_ABANDONED</unmanaged>
        public static readonly ResultDescriptor WaitAbandoned = new ResultDescriptor(0x00000080u, "General", "WAIT_ABANDONED", "Wait abandoned");

        /// <summary>
        ///   Represents a result code indicating the timeout of wait operation on a resource.
        /// </summary>
        /// <unmanaged>WAIT_TIMEOUT</unmanaged>
        public static readonly ResultDescriptor WaitTimeout = new ResultDescriptor(0x00000102u, "General", "WAIT_TIMEOUT", "Wait timeout");

        #endregion
    }
}
