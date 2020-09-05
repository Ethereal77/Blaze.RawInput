// Copyright © 2020 Infinisis

using System.Runtime.InteropServices;

using Blaze.Interop.Win32;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Provides helper methods to check results, error codes, and throw exceptions.
    /// </summary>
    internal static class ThrowHelper
    {
        public static void CheckResult(Result result)
        {
            if(result.Failure)
            {
                var win32errorCode = Marshal.GetLastWin32Error();
                throw new RawInputException(Result.FromWin32Error(win32errorCode), Marshal.GetExceptionForHR(win32errorCode));
            }
        }

        public static void CheckLastResult()
        {
            var win32errorCode = Marshal.GetLastWin32Error();
            throw new RawInputException(Result.FromWin32Error(win32errorCode), Marshal.GetExceptionForHR(win32errorCode));
        }
    }
}
