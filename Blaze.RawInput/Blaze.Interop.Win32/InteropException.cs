// Copyright © 2020 Infinisis

using System;

namespace Blaze.Interop.Win32
{
    /// <summary>
    ///   Represents errors that occur during native interop code.
    /// </summary>
    public class InteropException : Exception
    {
        /// <summary>
        ///   Gets the <see cref = "Result">Result code</see> for the exception if one exists.
        /// </summary>
        /// <value>The specific type of failure that occurred within the native code.</value>
        public Result ResultCode => Descriptor?.Result ?? HResult;

        /// <summary>
        ///   Gets the <see cref = "ResultDescriptor">Result descriptor</see> for the exception.
        /// </summary>
        /// <value>A description of the specific type of failure that occurred within the native code.</value>
        public ResultDescriptor Descriptor { get; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="InteropException"/> class.
        /// </summary>
        public InteropException() : base("An Interop exception occurred.")
        {
            Descriptor = Result.Fail;
            HResult = (int) Result.Fail;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="InteropException"/> class.
        /// </summary>
        /// <param name="errorCode">The result code that caused this exception.</param>
        public InteropException(Result errorCode) : this(ResultDescriptor.Find(errorCode))
        {
            HResult = (int) errorCode;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="InteropException"/> class.
        /// </summary>
        /// <param name="descriptor">The result descriptor.</param>
        public InteropException(ResultDescriptor descriptor) : base(descriptor.ToString())
        {
            Descriptor = descriptor;
            HResult = (int) descriptor.Result;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="InteropException"/> class.
        /// </summary>
        /// <param name="errorCode">The result code that caused this exception.</param>
        /// <param name="message">The message describing the exception.</param>
        public InteropException(Result errorCode, string message) : base(message)
        {
            Descriptor = ResultDescriptor.Find(errorCode);
            HResult = (int) errorCode;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="InteropException"/> class.
        /// </summary>
        /// <param name="message">The message describing the exception.</param>
        /// <param name="innerException">The inner exception that caused this exception.</param>
        public InteropException(string message, Exception innerException) : base(message, innerException)
        {
            Descriptor = Result.Fail;
            HResult = (int) Result.Fail;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="InteropException"/> class.
        /// </summary>
        /// <param name="message">The message describing the exception.</param>
        /// <param name="innerException">The inner exception that caused this exception.</param>
        public InteropException(Result errorCode, Exception innerException) : base(null, innerException)
        {
            Descriptor = ResultDescriptor.Find(errorCode);
            HResult = (int) errorCode;
        }
    }
}
