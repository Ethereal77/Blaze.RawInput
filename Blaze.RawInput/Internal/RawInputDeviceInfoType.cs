// Copyright © 2020 Infinisis

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Defines the data that can be retrieved with <see cref="RawInput.GetRawInputDeviceInfo"/>.
    /// </summary>
    /// <unmanaged>RAW_INPUT_DEVICE_INFO_TYPE</unmanaged>
    internal enum RawInputDeviceInfoType : uint
    {
        /// <summary>
        ///   Retrieve a buffer with preparsed data of a top-level collection.
        /// </summary>
        /// <unmanaged>RIDI_PREPARSEDDATA</unmanaged>
        PreparsedData = 0x20000005,

        /// <summary>
        ///   Retrieve a string containing the name of the device.
        /// </summary>
        /// <unmanaged>RIDI_DEVICENAME</unmanaged>
        DeviceName = 0x20000007,

        /// <summary>
        ///   Retrieve information about the device in a <see cref="RawDeviceInformation"/> structure.
        /// </summary>
        /// <unmanaged>RIDI_DEVICEINFO</unmanaged>
        DeviceInfo = 0x2000000B
    }
}
