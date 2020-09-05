// Copyright © 2020 Infinisis

using System;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Defines a structure containing information about a registered raw input device.
    /// </summary>
    /// <remarks>
    ///   Devices are registered with <see cref="RawInput.RegisterDevice(UsagePage, UsageId, DeviceFlags, IntPtr, RegisterDeviceOptions)"/>.
    /// </remarks>
    public readonly struct RegisteredDevice
    {
        private readonly RawInputDevice device;

        /// <summary>
        ///   Gets the top level collection <see cref="Framework.RawInput.UsagePage"/> for the raw input device.
        /// </summary>
        public readonly UsagePage UsagePage => device.UsagePage;

        /// <summary>
        ///   Gets the top level collection <see cref="Framework.RawInput.UsageId"/> for the raw input device.
        /// </summary>
        public readonly UsageId UsageId => device.UsageId;

        /// <summary>
        ///   Gets a combination of flags that determine how to interpret the information provided by <see cref="UsagePage"/>
        ///   and <see cref="UsageId"/>.
        /// </summary>
        /// <remarks>
        ///   By default, the operating system sends raw input from devices with the specified top level collection (TLC)
        ///   to the registered application as long as it has the window focus.
        /// </remarks>
        public readonly DeviceFlags Flags => device.Flags;

        /// <summary>
        ///   Gets the handle to the target window. If <see cref="IntPtr.Zero"/> it follows the keyboard focus.
        /// </summary>
        public readonly IntPtr TargetWindow => device.Target;


        /// <summary>
        ///   Initializes a new instance of the <see cref="RegisteredDevice"/> structure.
        /// </summary>
        /// <param name="rawInputDevice">The registered raw input device.</param>
        internal RegisteredDevice(ref RawInputDevice rawInputDevice)
        {
            device = rawInputDevice;
        }
    }
}
