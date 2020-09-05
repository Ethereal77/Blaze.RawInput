// Copyright © 2020 Infinisis

using System.Windows.Forms;

using Blaze.Interop.Win32;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Defines options that can be specified when initiating the message processing for raw input.
    /// </summary>
    public enum MessageProcessingOptions
    {
        /// <summary>
        ///   Processes the RawInput messages by installing a message filter with <see cref="Application.AddMessageFilter"/>.
        /// </summary>
        /// <remarks>
        ///   This is the default.
        /// </remarks>
        Default,

        /// <summary>
        ///   Processes the RawInput messages by installing a custom message filter with <see cref="MessageFilterHook"/>
        ///   instead of <see cref="Application.AddMessageFilter"/>.
        /// </summary>
        CustomFiltering
    }
}
