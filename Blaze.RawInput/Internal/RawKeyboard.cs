// Copyright © 2020 Infinisis

using System.Runtime.InteropServices;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Contains information about the state of the keyboard.
    /// </summary>
    /// <unmanaged>RAWKEYBOARD</unmanaged>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawKeyboard
    {
        /// <summary>
        ///   The scan code associated with a key press.
        /// </summary>
        public ushort MakeCode;

        /// <summary>
        ///   Flags for scan code information. It can be one or more of the values of <see cref="ScanCodeFlags"/>.
        /// </summary>
        public ScanCodeFlags Flags;

        /// <summary>
        ///   Reserved; must be zero.
        /// </summary>
        private readonly short reserved;

        /// <summary>
        ///   Windows message compatible virtual-key code.
        /// </summary>
        public ushort VirtualKey;

        /// <summary>
        ///   The corresponding window message, for example <strong>WM_KEYDOWN</strong>, <strong>WM_SYSKEYDOWN</strong>, and so forth.
        /// </summary>
        public KeyState Message;

        /// <summary>
        ///   The device-specific additional information for the event.
        /// </summary>
        public uint ExtraInformation;
    }
}
