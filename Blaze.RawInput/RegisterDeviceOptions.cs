// Copyright © 2020 Infinisis

using System.Windows.Forms;

using Blaze.Interop.Win32;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Defines options that can be specified when registering a device for raw input.
    /// </summary>
    public enum RegisterDeviceOptions
    {
        /// <summary>
        ///   Registers the device by installing a message filter with <see cref="Application.AddMessageFilter"/>
        ///   for RawInput message filtering.
        /// </summary>
        /// <remarks>
        ///   This is the default.
        /// </remarks>
        Default,

        /// <summary>
        ///   To disable message filtering.
        /// </summary>
        NoFiltering,

        /// <summary>
        ///   Registers the device by installing a custom message filter with <see cref="MessageFilterHook"/>
        ///   instead of <see cref="Application.AddMessageFilter"/>.
        /// </summary>
        CustomFiltering
    }
}
