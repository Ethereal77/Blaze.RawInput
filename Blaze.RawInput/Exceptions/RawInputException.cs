// Copyright © 2020 Infinisis

using System;

using Blaze.Interop.Win32;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Represents errors that occur during the configuration of devices or processing of events
    ///   with RawInput.
    /// </summary>
    public sealed class RawInputException : Exception
    {
        private readonly Result resultCode;

        public RawInputException(Result errorCode)
            : base($"RawInput call failed with error code [{errorCode}].")
        {
            resultCode = errorCode;
        }
    }
}
