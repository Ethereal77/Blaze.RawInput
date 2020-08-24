// Copyright © 2020 Infinisis

using System.Runtime.InteropServices;

using Blaze.Interop.Win32;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Describes the characteristics of a mouse for raw input handling.
    /// </summary>
    /// <remarks>
    ///   For the mouse, use <see cref="UsagePage.Generic"/> and <see cref="UsageId.GenericMouse"/>.
    /// </remarks>
    /// <unmanaged>RID_DEVICE_INFO_MOUSE</unmanaged>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct RawDeviceInformationMouse
    {
        /// <summary>
        ///   The identifier of the mouse device.
        /// </summary>
        /// <unmanaged>dwId</unmanaged>
        public readonly int Id;

        /// <summary>
        ///   The number of buttons for the mouse.
        /// </summary>
        /// <unmanaged>dwNumberOfButtons</unmanaged>
        public readonly int NumberOfButtons;

        /// <summary>
        ///   The number of data points per second. This information may not be applicable for every mouse device.
        /// </summary>
        /// <unmanaged>dwSampleRate</unmanaged>
        public readonly int SampleRate;

        /// <summary>
        ///   A value indicating whether the mouse has a wheel for horizontal scrolling.
        /// </summary>
        /// <unmanaged>fHasHorizontalWheel</unmanaged>
        public readonly Bool HasHorizontalWheel;
    }
}
