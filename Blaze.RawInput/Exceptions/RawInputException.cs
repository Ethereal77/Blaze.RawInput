// Copyright © 2020 Infinisis

using System;

using Blaze.Interop.Win32;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Represents errors that occur during the configuration of devices or processing of events with RawInput.
    /// </summary>
    public sealed class RawInputException : InteropException
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="RawInputException"/> class.
        /// </summary>
        public RawInputException() : base(Result.Fail, "A RawInput exception occurred.")
        { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RawInputException"/> class.
        /// </summary>
        /// <param name="result">The result code that caused this exception.</param>
        public RawInputException(Result errorCode)
            : base(errorCode, $"RawInput call failed with error code [{errorCode}].")
        { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RawInputException"/> class.
        /// </summary>
        /// <param name="message">The message describing the exception.</param>
        public RawInputException(string message) : base(Result.Fail, message)
        { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RawInputException"/> class.
        /// </summary>
        /// <param name="result">The result code that caused this exception.</param>
        /// <param name="message">The message describing the exception.</param>
        public RawInputException(Result result, string message) : base(result, message)
        { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RawInputException"/> class.
        /// </summary>
        /// <param name="errorCode">The result code that caused this exception.</param>
        /// <param name="innerException">The inner exception that caused this exception.</param>
        public RawInputException(Result errorCode, Exception innerException)
            : base(errorCode, innerException)
        { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RawInputException"/> class.
        /// </summary>
        /// <param name="message">The message describing the exception.</param>
        /// <param name="innerException">The inner exception that caused this exception.</param>
        public RawInputException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
