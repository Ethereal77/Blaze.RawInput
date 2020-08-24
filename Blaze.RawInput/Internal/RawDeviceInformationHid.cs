// Copyright © 2020 Infinisis

using System.Runtime.InteropServices;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Describes the characteristics of a Human Input Device (HID) for raw input handling.
    /// </summary>
    /// <unmanaged>RID_DEVICE_INFO_HID</unmanaged>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct RawDeviceInformationHid
    {
        /// <summary>
        ///   The vendor identifier for the HID.
        /// </summary>
        /// <unmanaged>dwVendorId</unmanaged>
        public readonly int VendorId;

        /// <summary>
        ///   The product identifier for the HID.
        /// </summary>
        /// <unmanaged>dwProductId</unmanaged>
        public readonly int ProductId;

        /// <summary>
        ///   The version number for the HID.
        /// </summary>
        /// <unmanaged>dwVersionNumber</unmanaged>
        public readonly int VersionNumber;

        /// <summary>
        ///   The top-level collection Usage Page for the device.
        /// </summary>
        /// <unmanaged>usUsagePage</unmanaged>
        public readonly UsagePage UsagePage;

        /// <summary>
        ///   The top-level collection Usage for the device.
        /// </summary>
        /// <unmanaged>usUsage</unmanaged>
        public readonly UsageId Usage;
    }
}
