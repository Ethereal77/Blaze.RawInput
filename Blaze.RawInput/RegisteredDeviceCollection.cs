// Copyright © 2020 Infinisis

using System;
using System.Collections;
using System.Collections.Generic;

using static Blaze.Framework.RawInput.RawInput;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Represents a collection of RawInput devices registered by the application.
    /// </summary>
    public sealed class RegisteredDeviceCollection : IEnumerable<RegisteredDevice>, IReadOnlyCollection<RegisteredDevice>
    {
        private RegisteredDevice[] registeredDevices;

        /// <summary>
        ///   Initializes a new instance of the <see cref="RegisteredDeviceCollection"/> class.
        /// </summary>
        internal RegisteredDeviceCollection()
        {
            registeredDevices = GetRegisteredDevices();
        }

        /// <summary>
        ///   Populates a list of the registered devices.
        /// </summary>
        /// <returns>An array of <see cref="RegisteredDevice"/>.</returns>
        private static RegisteredDevice[] GetRegisteredDevices()
        {
            // Query the number of devices registered for RawInput
            int numDevices = GetRegisteredRawInputDeviceCount();
            if (numDevices == 0)
                return Array.Empty<RegisteredDevice>();

            // Query info about the devices
            Span<RawInputDevice> rawInputDevices = stackalloc RawInputDevice[numDevices];
            GetRegisteredRawInputDevices(rawInputDevices);

            var devices = new RegisteredDevice[numDevices];
            for (int index = 0; index < numDevices; ++index)
            {
                devices[index] = new RegisteredDevice(ref rawInputDevices[index]);
            }

            return devices;
        }

        /// <summary>
        ///   Gets information about a registered device.
        /// </summary>
        /// <param name="index">Index of the device.</param>
        /// <returns>An instance of <see cref="RegisteredDevice"/> with information about the registered device.</returns>
        public RegisteredDevice this[int index] => registeredDevices[index];

        /// <summary>
        ///   Gets the number of available devices contained in the collection.
        /// </summary>
        /// <value>The number of available devices contained in this <see cref="DeviceInfoCollection"/>.</value>
        public int Count => registeredDevices.Length;

        IEnumerator<RegisteredDevice> IEnumerable<RegisteredDevice>.GetEnumerator()
        {
            foreach (var device in registeredDevices)
            {
                yield return device;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => registeredDevices.GetEnumerator();

        /// <summary>
        ///   Notifies the collection that a device has been registered or removed so the collection can update itself.
        ///   This method is called whenever a device is registered or unregistered by the application by calling the
        ///   <see cref="RawInput.RegisterDevice(UsagePage, UsageId, DeviceFlags)"/> method.
        /// </summary>
        internal void Update()
        {
            registeredDevices = GetRegisteredDevices();
        }
    }
}
