// Copyright © 2020 Infinisis

using System;
using System.Runtime.InteropServices;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Defines information for the raw input devices.
    /// </summary>
    /// <remarks>
    ///   If <see cref="DeviceFlags.NoLegacy"/> is set for a mouse or a keyboard, the system does not generate
    ///   any legacy message for that device for the application.
    ///   If <see cref="DeviceFlags.Remove"/> is set and <see cref="Target"/> is not set to
    ///   <see cref="IntPtr.Zero"/>, then parameter validation will fail.
    /// </remarks>
    /// <unmanaged>RAWINPUTDEVICE</unmanaged>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputDevice
    {
        /// <summary>
        ///   Top level collection <see cref="Framework.RawInput.UsagePage"/> for the raw input device.
        /// </summary>
        /// <unmanaged>usUsagePage</unmanaged>
        public UsagePage UsagePage;

        /// <summary>
        ///   Top level collection <see cref="Framework.RawInput.UsageId"/> for the raw input device.
        /// </summary>
        /// <unmanaged>usUsage</unmanaged>
        public UsageId UsageId;

        /// <summary>
        ///   A combination of flags that determine how to interpret the information provided by <see cref="UsagePage"/>
        ///   and <see cref="UsageId"/>. It can be zero (the default) or a combination of values.
        /// </summary>
        /// <remarks>
        ///   By default, the operating system sends raw input from devices with the specified top level collection (TLC)
        ///   to the registered application as long as it has the window focus.
        /// </remarks>
        /// <unmanaged>dwFlags</unmanaged>
        public DeviceFlags Flags;

        /// <summary>
        ///   A handle to the target window. If <see cref="IntPtr.Zero"/> it follows the keyboard focus.
        /// </summary>
        /// <unmanaged>hwndTarget</unmanaged>
        public IntPtr Target;
    }
}
