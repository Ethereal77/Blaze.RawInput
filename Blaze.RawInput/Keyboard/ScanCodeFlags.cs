// Copyright © 2020 Infinisis

using System;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Defines flags for keyboard scan code information.
    /// </summary>
    [Flags]
    public enum ScanCodeFlags : ushort
    {
        /// <summary>
        ///   The key is down.
        /// </summary>
        /// <unmanaged>RI_KEY_MAKE</unmanaged>
        Make = 0,

        /// <summary>
        ///   The key is up.
        /// </summary>
        /// <unmanaged>RI_KEY_BREAK</unmanaged>
        Break = 1,

        /// <summary>
        ///   The scan code has the E0 prefix.
        /// </summary>
        /// <unmanaged>RI_KEY_E0</unmanaged>
        E0 = 2,

        /// <summary>
        ///   The scan code has the E1 prefix.
        /// </summary>
        /// <unmanaged>RI_KEY_E1</unmanaged>
        E1 = 4
    }
}
