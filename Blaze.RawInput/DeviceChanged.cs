// Copyright © 2020 Infinisis

using System;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Represents the method that handles the <see cref="RawInput.DeviceChanged"/> event for
    ///   being norified when a device is added to or removed from the system.
    /// </summary>
    /// <param name="deviceHandle">Handle of the device that has changed.</param>
    /// <param name="deviceChange">The type of change.</param>
    public delegate void DeviceChangedEventHandler(IntPtr deviceHandle, DeviceChange deviceChange);

    /// <summary>
    ///   Defines the device change notifications that can be received when a device
    ///   is registered for raw input with the flag <see cref="DeviceFlags.DeviceNotify"/>.
    /// </summary>
    public enum DeviceChange
    {
        /// <summary>
        ///   A new device has been added to the system.
        /// </summary>
        /// <unmanaged>GIDC_ARRIVAL</unmanaged>
        Arrival = 1,

        /// <summary>
        ///   A device has been removed from the system.
        /// </summary>
        /// <unmanaged>GIDC_REMOVAL</unmanaged>
        Removal = 2
    }
}
