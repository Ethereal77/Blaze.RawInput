// Copyright © 2020 Infinisis

using System.Runtime.InteropServices;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Defines the raw input data coming from any device.
    /// </summary>
    /// <unmanaged>RID_DEVICE_INFO</unmanaged>
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct RawDeviceInformation
    {
        /// <summary>
        ///   The size of the <see cref="RawDeviceInformation"/> structure, in bytes.
        /// </summary>
        /// <unmanaged>cbSize</unmanaged>
        [FieldOffset(0)]
        public readonly int Size;

        /// <summary>
        ///   The type of raw input data.
        ///   This member can be one of the following values:
        ///   <list type="bullet">
        ///     <item><see cref="DeviceType.HumanInputDevice"/> if data comes from an HID that is not a keyboard or a mouse.</item>
        ///     <item><see cref="DeviceType.Keyboard"/> if data comes from a keyboard.</item>
        ///     <item><see cref="DeviceType.Mouse"/> if data comes from a mouse.</item>
        ///   </list>
        /// </summary>
        /// <unmanaged>dwType</unmanaged>
        [FieldOffset(4)]
        public readonly DeviceType Type;

        /// <summary>
        ///   If <see cref="Type"/> is <see cref="DeviceType.Mouse"/>, this is the <see cref="RawDeviceInformationMouse"/> structure
        ///   that describes the mouse characteristics.
        /// </summary>
        /// <unmanaged>mouse</unmanaged>
        [FieldOffset(8)]
        public readonly RawDeviceInformationMouse Mouse;

        /// <summary>
        ///   If <see cref="Type"/> is <see cref="DeviceType.Keyboard"/>, this is the <see cref="RawDeviceInformationKeyboard"/> structure
        ///   that describes the keyboard characteristics.
        /// </summary>
        /// <unmanaged>keyboard</unmanaged>
        [FieldOffset(8)]
        public readonly RawDeviceInformationKeyboard Keyboard;

        /// <summary>
        ///   If <see cref="Type"/> is <see cref="DeviceType.HumanInputDevice"/>, this is the <see cref="RawDeviceInformationHid"/>
        ///   structure that describes the HID device characteristics.
        /// </summary>
        /// <unmanaged>hid</unmanaged>
        [FieldOffset(8)]
        public readonly RawDeviceInformationHid Hid;
    }
}
