// Copyright © 2020 Infinisis

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Defines the valid usage identifiers in a usage page of related HIDs for RawInput.
    /// </summary>
    public enum UsageId : ushort
    {
        /// <summary>
        ///   A pointer device.
        /// </summary>
        GenericPointer = 0x01,

        /// <summary>
        ///   A mouse.
        /// </summary>
        GenericMouse = 0x02,

        /// <summary>
        ///   A joystick.
        /// </summary>
        GenericJoystick = 0x04,

        /// <summary>
        ///   A gamepad.
        /// </summary>
        GenericGamepad = 0x05,

        /// <summary>
        ///   A keyboard.
        /// </summary>
        GenericKeyboard = 0x06,

        /// <summary>
        ///   A keypad.
        /// </summary>
        GenericKeypad = 0x07,

        /// <summary>
        ///   A controller device with multiple axis of control.
        /// </summary>
        GenericMultiAxisController = 0x08
    }
}
