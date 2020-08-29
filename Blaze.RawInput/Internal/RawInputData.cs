// Copyright © 2020 Infinisis

using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Represents a RawInput packet from a device.
    /// </summary>
    /// <remarks>
    ///   The handle to this structure is passed in the <see cref="Message.LParam"/>
    ///   parameter of the <c>WM_INPUT</c> message.
    /// </remarks>
    /// <unmanaged>RAWINPUT</unmanaged>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct RawInputData
    {
        /// <summary>
        ///   Header of the RawInput data packet.
        /// </summary>
        /// <unmanaged>header</unmanaged>
        public readonly RawInputHeader Header;

        /// <summary>
        ///   Data of the RawInput packet.
        /// </summary>
        /// <unmanaged>data</unmanaged>
        public readonly RawInputInnerData Data;

        #region Static properties

        /// <summary>
        ///   Size of the <see cref="RawInputData"/> structure, in bytes.
        /// </summary>
        public static readonly uint Size = (uint) Unsafe.SizeOf<RawInputData>();

        #endregion
    }

    /// <summary>
    ///   Contains the header information that is part of the raw input data.
    /// </summary>
    /// <remarks>
    ///   To get more information on the device, use <see cref="Device"/> in a call to
    ///   <see cref="RawInput.GetRawInputDeviceInfo"/>.
    /// </remarks>
    /// <unmanaged>RAWINPUTHEADER</unmanaged>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct RawInputHeader
    {
        /// <summary>
        ///   The type of raw input device.
        /// </summary>
        /// <unmanaged>dwType</unmanaged>
        public readonly DeviceType Type;

        /// <summary>
        ///   The size of the entire input packet of data, in bytes.
        /// </summary>
        /// <remarks>
        ///   This includes <see cref="RawInputData"/> plus possible extra input reports in the <see cref="RawHid"/>
        ///   variable length array.
        /// </remarks>
        /// <unmanaged>dwSize</unmanaged>
        public readonly int Size;

        /// <summary>
        ///   The handle of the device generating the raw input data.
        /// </summary>
        /// <unmanaged>hDevice</unmanaged>
        public readonly IntPtr Device;

        /// <summary>
        ///   The value passed in the  <c>wParam</c> parameter of the <c>WM_INPUT</c> message.
        /// </summary>
        /// <unmanaged>wParam</unmanaged>
        public readonly IntPtr WParam;

        #region Static properties

        /// <summary>
        ///   Size of the <see cref="RawInputHeader"/> structure, in bytes.
        /// </summary>
        public static readonly uint HeaderSize = (uint) Unsafe.SizeOf<RawInputHeader>();

        #endregion
    }

    /// <summary>
    ///   Contains the received input that is part of the raw input data.
    /// </summary>
    /// <unmanaged>RAWINPUT_INNER_0</unmanaged>
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct RawInputInnerData
    {
        /// <summary>
        ///   If the data comes from a mouse, this is the raw input data.
        /// </summary>
        /// <unmanaged>mouse</unmanaged>
        [FieldOffset(0)]
        public readonly RawMouse Mouse;

        /// <summary>
        ///   If the data comes from a keyboard, this is the raw input data.
        /// </summary>
        /// <unmanaged>keyboard</unmanaged>
        [FieldOffset(0)]
        public readonly RawKeyboard Keyboard;

        /// <summary>
        ///   If the data comes from an HID, this is the raw input data.
        /// </summary>
        /// <unmanaged>hid</unmanaged>
        [FieldOffset(0)]
        public readonly RawHid Hid;
    }
}
