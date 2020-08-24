// Copyright © 2020 Infinisis

using System;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Describes the characteristics of a Human Input Device (HID) for raw input handling.
    /// </summary>
    public sealed class HidInfo : DeviceInfo
    {
        /// <summary>
        ///   Gets or sets the vendor identifier of the device.
        /// </summary>
        /// <unmanaged>unsigned int dwVendorId</unmanaged>
        public int VendorId { get; private set; }

        /// <summary>
        ///   Gets or sets the product identifier of the device.
        /// </summary>
        /// <unmanaged>unsigned int dwProductId</unmanaged>
        public int ProductId { get; private set; }

        /// <summary>
        ///   Gets or sets the version number of the device.
        /// </summary>
        /// <unmanaged>unsigned int dwVersionNumber</unmanaged>
        public int VersionNumber { get; private set; }

        /// <summary>
        ///   Gets or sets the usage page of the device.
        /// </summary>
        /// <unmanaged>HID_USAGE_PAGE usUsagePage</unmanaged>
        public UsagePage UsagePage { get; private set; }

        /// <summary>
        ///   Gets or sets the usage identifier of the device.
        /// </summary>
        /// <unmanaged>HID_USAGE_ID usUsage</unmanaged>
        public UsageId Usage { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="HidInfo" /> class.
        /// </summary>
        /// <param name="rawDeviceInfo">The raw device info.</param>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="deviceHandle">The device handle.</param>
        internal HidInfo(in RawDeviceInformation rawDeviceInfo, string deviceName, IntPtr deviceHandle)
          : base(in rawDeviceInfo, deviceName, deviceHandle)
        {
            VendorId = rawDeviceInfo.Hid.VendorId;
            ProductId = rawDeviceInfo.Hid.ProductId;
            VersionNumber = rawDeviceInfo.Hid.VersionNumber;
            UsagePage = rawDeviceInfo.Hid.UsagePage;
            Usage = rawDeviceInfo.Hid.Usage;
        }
    }
}
