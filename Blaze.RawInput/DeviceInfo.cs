// Copyright © 2020 Infinisis

using System;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Describes the characteristics of a raw input device.
    /// </summary>
    /// <unmanaged>RID_DEVICE_INFO</unmanaged>
    public abstract class DeviceInfo
    {
        /// <summary>
        ///   Gets the name of the device.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///   Gets the type of the device.
        /// </summary>
        public DeviceType Type { get; private set; }

        /// <summary>
        ///   Gets the handle that identifies the device.
        /// </summary>
        public IntPtr Handle { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="DeviceInfo" /> class.
        /// </summary>
        /// <param name="rawDeviceInfo">The raw device info.</param>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="deviceHandle">The device handle.</param>
        protected private DeviceInfo(in RawDeviceInformation rawDeviceInfo, string deviceName, IntPtr deviceHandle)
        {
            Name = deviceName;
            Handle = deviceHandle;
            Type = rawDeviceInfo.Type;
        }


        /// <summary>
        ///   Creates a specific <see cref="DeviceInfo"/> from its raw device information.
        /// </summary>
        /// <param name="rawDeviceInfo">The raw device info.</param>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="deviceHandle">The device handle.</param>
        /// <returns></returns>
        internal static DeviceInfo Create(ref RawDeviceInformation rawDeviceInfo, string deviceName, IntPtr deviceHandle) =>
            rawDeviceInfo.Type switch
            {
                DeviceType.Mouse => new MouseInfo(in rawDeviceInfo, deviceName, deviceHandle),
                DeviceType.Keyboard => new KeyboardInfo(in rawDeviceInfo, deviceName, deviceHandle),
                DeviceType.HumanInputDevice => new HidInfo(in rawDeviceInfo, deviceName, deviceHandle),

                _ => throw new RawInputException($"Unsupported Device Type [{(int) rawDeviceInfo.Type}].")
            };
    }
}
