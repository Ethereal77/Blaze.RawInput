// Copyright © 2020 Infinisis

using System;
using System.Runtime.InteropServices;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Contains information about a raw input device.
    /// </summary>
    /// <unmanaged>RAWINPUTDEVICELIST</unmanaged>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct RawInputDeviceList
    {
        /// <summary>
        ///   A handle to the raw input device.
        /// </summary>
        /// <unmanaged>hDevice</unmanaged>
        public readonly IntPtr Device;

        /// <summary>
        ///   The type of device.
        /// </summary>
        /// <unmanaged>dwType</unmanaged>
        public readonly DeviceType Type;
    }
}
