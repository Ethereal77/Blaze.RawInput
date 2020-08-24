// Copyright © 2020 Infinisis

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Defines the usage pages of related HIDs for RawInput.
    /// </summary>
    public enum UsagePage : ushort
    {
        /// <summary>
        ///   Generic desktop controls.
        /// </summary>
        Generic = 0x01,

        /// <summary>
        ///   Game controls.
        /// </summary>
        Game = 0x05,

        /// <summary>
        ///   LED indicators.
        /// </summary>
        Led = 0x08,

        /// <summary>
        ///   Buttons.
        /// </summary>
        Button = 0x09
    }
}
