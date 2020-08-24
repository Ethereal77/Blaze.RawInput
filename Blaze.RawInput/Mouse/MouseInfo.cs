// Copyright © 2020 Infinisis

using System;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Describes the characteristics of a mouse for raw input handling.
    /// </summary>
    /// <unmanaged>RID_DEVICE_INFO_MOUSE</unmanaged>
    public sealed class MouseInfo : DeviceInfo
    {
        /// <summary>
        ///   Gets the device identifier.
        /// </summary>
        /// <unmanaged>unsigned int dwId</unmanaged>
        public int Id { get; set; }

        /// <summary>
        ///   Gets the number of buttons of the mouse.
        /// </summary>
        /// <unmanaged>unsigned int dwNumberOfButtons</unmanaged>
        public int ButtonCount { get; set; }

        /// <summary>
        ///   Gets the sample rate of the mouse, i.e. the number of datapoints per second.
        /// </summary>
        /// <unmanaged>unsigned int dwSampleRate</unmanaged>
        public int SampleRate { get; set; }

        /// <summary>
        ///   Gets a value indicating whether this mouse has a horizontal wheel.
        /// </summary>
        /// <value><c>true</c> if this mouse has horizontal wheel; otherwise, <c>false</c>.</value>
        /// <unmanaged>BOOL fHasHorizontalWheel</unmanaged>
        public bool HasHorizontalWheel { get; set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="MouseInfo"/> class.
        /// </summary>
        /// <param name="rawDeviceInfo">The raw device info.</param>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="deviceHandle">The device handle.</param>
        internal MouseInfo(in RawDeviceInformation rawDeviceInfo, string deviceName, IntPtr deviceHandle)
          : base(in rawDeviceInfo, deviceName, deviceHandle)
        {
            Id = rawDeviceInfo.Mouse.Id;
            ButtonCount = rawDeviceInfo.Mouse.NumberOfButtons;
            SampleRate = rawDeviceInfo.Mouse.SampleRate;
            HasHorizontalWheel = rawDeviceInfo.Mouse.HasHorizontalWheel;
        }
    }
}
