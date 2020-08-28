// Copyright © 2020 Infinisis

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
        private static unsafe Dictionary<IntPtr, DeviceInfo> GetAvailableDevices()
        {
            // Query the number of devices available for RawInput
            uint numDevices = 0;
            GetRawInputDeviceList(null, ref numDevices, (uint) Unsafe.SizeOf<RawInputDeviceList>());
            if (numDevices == 0)
                return new Dictionary<IntPtr, DeviceInfo>(0);

            // Query info about the devices
            var rawInputDevices = new RawInputDeviceList[numDevices];
            GetRawInputDeviceList(rawInputDevices, ref numDevices, (uint) Unsafe.SizeOf<RawInputDeviceList>());
            var devices = new Dictionary<IntPtr, DeviceInfo>((int) numDevices);
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
        private static unsafe DeviceInfo GetDeviceInfo(IntPtr deviceHandle)
        {
            // Get the name of the device
            uint nameLength = 0;
            GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoType.DeviceName, IntPtr.Zero, ref nameLength);
            char* nameChars = stackalloc char[(int)nameLength];
            GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoType.DeviceName, (IntPtr) nameChars, ref nameLength);

            int indexCh = 0;
            do { } while (indexCh <= nameLength && nameChars[indexCh++] != 0);
            string deviceName = new string(nameChars, 0, indexCh == 0 ? 0 : indexCh - 1);

            // Get device info
            uint infoLength = 0;
            GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoType.DeviceInfo, IntPtr.Zero, ref infoLength);
            byte* infoBytes = stackalloc byte[(int)infoLength];
            GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoType.DeviceInfo, (IntPtr) infoBytes, ref infoLength);

            return DeviceInfo.Create(ref *(RawDeviceInformation*) infoBytes, deviceName, deviceHandle);
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
