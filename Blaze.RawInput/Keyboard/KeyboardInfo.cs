// Copyright © 2020 Infinisis

using System;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Describes the characteristics of a keyboard for raw input handling.
    /// </summary>
    /// <unmanaged>RID_DEVICE_INFO_KEYBOARD</unmanaged>
    public sealed class KeyboardInfo : DeviceInfo
    {
        /// <summary>
        ///   Gets the type of the keyboard.
        /// </summary>
        /// <unmanaged>unsigned int dwType</unmanaged>
        public int KeyboardType { get; private set; }

        /// <summary>
        ///   Gets the subtype of the keyboard.
        /// </summary>
        /// <unmanaged>unsigned int dwSubType</unmanaged>
        public int Subtype { get; private set; }

        /// <summary>
        ///   Gets the keyboard mode.
        /// </summary>
        /// <unmanaged>unsigned int dwKeyboardMode</unmanaged>
        public int KeyboardMode { get; private set; }

        /// <summary>
        ///   Gets the number of function keys.
        /// </summary>
        /// <unmanaged>unsigned int dwNumberOfFunctionKeys</unmanaged>
        public int FunctionKeyCount { get; private set; }

        /// <summary>
        ///   Gets the number of indicators.
        /// </summary>
        /// <unmanaged>unsigned int dwNumberOfIndicators</unmanaged>
        public int IndicatorCount { get; private set; }

        /// <summary>
        ///   Gets the total number of keys.
        /// </summary>
        /// <unmanaged>unsigned int dwNumberOfKeysTotal</unmanaged>
        public int TotalKeyCount { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="KeyboardInfo"/> class.
        /// </summary>
        /// <param name="rawDeviceInfo">The raw device info.</param>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="deviceHandle">The device handle.</param>
        internal KeyboardInfo(in RawDeviceInformation rawDeviceInfo, string deviceName, IntPtr deviceHandle)
          : base(in rawDeviceInfo, deviceName, deviceHandle)
        {
            KeyboardType = rawDeviceInfo.Keyboard.Type;
            Subtype = rawDeviceInfo.Keyboard.SubType;
            KeyboardMode = rawDeviceInfo.Keyboard.KeyboardMode;
            FunctionKeyCount = rawDeviceInfo.Keyboard.NumberOfFunctionKeys;
            IndicatorCount = rawDeviceInfo.Keyboard.NumberOfIndicators;
            TotalKeyCount = rawDeviceInfo.Keyboard.NumberOfKeysTotal;
        }
    }
}
