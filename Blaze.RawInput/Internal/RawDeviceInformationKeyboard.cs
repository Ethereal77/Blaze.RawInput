// Copyright © 2020 Infinisis

using System.Runtime.InteropServices;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Describes the characteristics of a keyboard for raw input handling.
    /// </summary>
    /// <remarks>
    ///   For the keyboard, use <see cref="UsagePage.Generic"/> and <see cref="UsageId.GenericKeyboard"/>.
    /// </remarks>
    /// <unmanaged>RID_DEVICE_INFO_KEYBOARD</unmanaged>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct RawDeviceInformationKeyboard
    {
        /// <summary>
        ///   The type of the keyboard.
        /// </summary>
        /// <unmanaged>dwType</unmanaged>
        public readonly int Type;

        /// <summary>
        ///   The subtype of the keyboard.
        /// </summary>
        /// <unmanaged>dwSubType</unmanaged>
        public readonly int SubType;

        /// <summary>
        ///   The scan code mode.
        /// </summary>
        /// <unmanaged>dwKeyboardMode</unmanaged>
        public readonly int KeyboardMode;

        /// <summary>
        ///   The number of function keys on the keyboard.
        /// </summary>
        /// <unmanaged>dwNumberOfFunctionKeys</unmanaged>
        public readonly int NumberOfFunctionKeys;

        /// <summary>
        ///   The number of LED indicators on the keyboard.
        /// </summary>
        /// <unmanaged>dwNumberOfIndicators</unmanaged>
        public readonly int NumberOfIndicators;

        /// <summary>
        ///   The total number of keys on the keyboard.
        /// </summary>
        /// <unmanaged>dwNumberOfKeysTotal</unmanaged>
        public readonly int NumberOfKeysTotal;
    }
}
