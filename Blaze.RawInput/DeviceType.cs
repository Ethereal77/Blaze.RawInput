// Copyright © 2020 Infinisis

namespace Blaze.Framework.RawInput
{
  /// <summary>
  ///   Defines the types of the input devices available for RawInput.
  /// </summary>
  /// <unmanaged>RAW_DEVICE_TYPE</unmanaged>
  public enum DeviceType
  {
    /// <summary>
    ///   A mouse device.
    /// </summary>
    /// <unmanaged>RIM_TYPEMOUSE</unmanaged>
    Mouse = 0,

    /// <summary>
    ///   A keyboard device.
    /// </summary>
    /// <unmanaged>RIM_TYPEKEYBOARD</unmanaged>
    Keyboard = 1,

    /// <summary>
    ///   A Human Input Device (HID) other than keyboard and mouse.
    /// </summary>
    /// <unmanaged>RIM_TYPEHID</unmanaged>
    HumanInputDevice = 2
  }
}
