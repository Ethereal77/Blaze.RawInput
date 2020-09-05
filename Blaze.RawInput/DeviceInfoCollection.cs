// Copyright © 2020 Infinisis

using System;
using System.Collections;
using System.Collections.Generic;

using static Blaze.Framework.RawInput.RawInput;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Represents a collection of available RawInput devices.
    /// </summary>
    public sealed class DeviceInfoCollection : IEnumerable<DeviceInfo>, IReadOnlyCollection<DeviceInfo>
    {
        private readonly Dictionary<IntPtr, DeviceInfo> devicesByHandle;

        /// <summary>
        ///   Initializes a new instance of the <see cref="DeviceInfoCollection"/> class.
        /// </summary>
        internal DeviceInfoCollection()
        {
            devicesByHandle = GetAvailableDevices();
        }

        /// <summary>
        ///   Populates a dictionary of the available devices associated with their handle.
        /// </summary>
        /// <returns>A dictionary of <see cref="DeviceInfo"/> associated with its <see cref="DeviceInfo.Handle"/>.</returns>
        private static Dictionary<IntPtr, DeviceInfo> GetAvailableDevices()
        {
            // Query the number of devices available for RawInput
            int numDevices = GetRawInputDeviceCount();
            if (numDevices == 0)
                return new Dictionary<IntPtr, DeviceInfo>(0);

            // Query info about the devices
            Span<RawInputDeviceList> rawInputDevices = stackalloc RawInputDeviceList[numDevices];
            GetRawInputDeviceList(rawInputDevices);

            var devices = new Dictionary<IntPtr, DeviceInfo>(numDevices);
            for (int index = 0; index < numDevices; ++index)
            {
                IntPtr deviceHandle = rawInputDevices[index].Device;

                devices[deviceHandle] = GetDeviceInfo(deviceHandle);
            }

            return devices;
        }

        /// <summary>
        ///   Returns an instance of <see cref="DeviceInfo"/> with information about the device.
        /// </summary>
        /// <param name="deviceHandle">Handle of the device.</param>
        /// <returns>An instance of <see cref="DeviceInfo"/>.</returns>
        private static DeviceInfo GetDeviceInfo(IntPtr deviceHandle)
        {
            string deviceName = GetRawInputDeviceName(deviceHandle);

            int deviceInfoLength = GetRawInputDeviceInfoSize(deviceHandle);
            Span<byte> deviceInfoData = stackalloc byte[deviceInfoLength];
            ref RawDeviceInformation deviceInfo = ref GetRawInputDeviceInfo(deviceHandle, deviceInfoData);

            return DeviceInfo.Create(ref deviceInfo, deviceName, deviceHandle);
        }

        /// <summary>
        ///   Gets information about a device based on the provided handle.
        /// </summary>
        /// <param name="deviceHandle">Handle of the device.</param>
        /// <returns>
        ///   An instance of <see cref="DeviceInfo"/> with information about the device. If the <see cref="DeviceInfo.Type"/>
        ///   is <see cref="DeviceType.Keyboard"/>, <see cref="DeviceType.Mouse"/> or <see cref="DeviceType.HumanInputDevice"/>,
        ///   you can cast the value returned from this property to <see cref="KeyboardInfo"/>, <see cref="MouseInfo"/> or
        ///   <see cref="HidInfo"/>, respectively, to obtain more specific information about the device.
        /// </returns>
        public DeviceInfo this[IntPtr deviceHandle] => devicesByHandle?[deviceHandle];

        /// <summary>
        ///   Gets the number of available devices contained in the collection.
        /// </summary>
        /// <value>The number of available devices contained in this <see cref="DeviceInfoCollection"/>.</value>
        public int Count => devicesByHandle.Count;

        /// <summary>
        ///   Returns an enumerator that iterates through the available devices in the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public Dictionary<IntPtr, DeviceInfo>.ValueCollection.Enumerator GetEnumerator() => devicesByHandle.Values.GetEnumerator();

        IEnumerator<DeviceInfo> IEnumerable<DeviceInfo>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        ///   Notifies the collection that a device has been added to or removed from the system so the
        ///   collection can update itself.
        ///   This method is called whenever a <see cref="DeviceChanged"/> event occurs but before any
        ///   user code is invoked.
        /// </summary>
        /// <param name="deviceHandle">Handle of the device that has changed.</param>
        /// <param name="change">The change notification.</param>
        internal void NotifyDeviceChanged(IntPtr deviceHandle, DeviceChange change)
        {
            switch (change)
            {
                case DeviceChange.Arrival:
                    devicesByHandle[deviceHandle] = GetDeviceInfo(deviceHandle);
                    break;

                case DeviceChange.Removal:
                    devicesByHandle.Remove(deviceHandle);
                    break;
            }
        }
    }
}
