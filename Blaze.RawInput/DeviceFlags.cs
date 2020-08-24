// Copyright © 2020 Infinisis

using System;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Defines flags to specify how to interpret the information provided by <see cref="UsagePage"/>
    ///   and <see cref="UsageId"/>.
    /// </summary>
    [Flags]
    public enum DeviceFlags
    {
        /// <unmanaged>None</unmanaged>
        None = 0,

        /// <summary>
        ///   Remove the top level collection from the inclusion list. This tells the operating system to
        ///   stop reading from a device which matches the top level collection.
        /// </summary>
        /// <unmanaged>RIDEV_REMOVE</unmanaged>
        Remove = 1,

        /// <summary>
        ///   Specify the top level collections to exclude when reading a complete usage page.
        /// </summary>
        /// <remarks>
        ///   This flag only affects a top level collection whose usage page is already specified
        ///   with <see cref="PageOnly"/>.
        /// </remarks>
        /// <unmanaged>RIDEV_EXCLUDE</unmanaged>
        Exclude = 0x00000010,

        /// <summary>
        ///   Specify all devices whose top level collection is from the specified <see cref="Multimedia.UsagePage"/>.
        /// </summary>
        /// <remarks>
        ///   When using this flag, the <see cref="Multimedia.UsageId"/> must be <see cref="Multimedia.UsageId.KeyboardNoevent"/>.
        /// </remarks>
        /// <unmanaged>RIDEV_PAGEONLY</unmanaged>
        PageOnly = 0x00000020,

        /// <summary>
        ///   Prevent any devices specified by <see cref="Multimedia.UsagePage"/> or <see cref="Multimedia.UsageId"/>
        ///   from generating legacy Windows messages.
        /// </summary>
        /// <remarks>
        ///   This is only valid for mouse and keyboard.
        /// </remarks>
        /// <unmanaged>RIDEV_NOLEGACY</unmanaged>
        NoLegacy = PageOnly | Exclude,

        /// <summary>
        ///   Enable the caller to receive the input even when the caller is not in the foreground.
        /// </summary>
        /// <remarks>
        ///   Even when using this flag, the target window handle must be specified.
        /// </remarks>
        /// <unmanaged>RIDEV_INPUTSINK</unmanaged>
        InputSink = 0x00000100,

        /// <summary>
        ///   Mouse button clicks do not activate other windows.
        /// </summary>
        /// <remarks>
        ///   This flag can only be specified when <see cref="NoLegacy"/> is specified for a mouse device.
        /// </remarks>
        /// <unmanaged>RIDEV_CAPTUREMOUSE</unmanaged>
        CaptureMouse = 0x00000200,

        /// <summary>
        ///   The application-defined keyboard device hotkeys are not handled. However, the system hotkeys
        ///   (Alt + Tab, Ctrl + Alt + Del, etc) are still handled.
        /// </summary>
        /// <unmanaged>RIDEV_NOHOTKEYS</unmanaged>
        NoHotKeys = CaptureMouse,

        /// <summary>
        ///   The application command keys are handled.
        /// </summary>
        /// <remarks>
        ///   This flag can only be specified when <see cref="NoLegacy"/> is specified for a keyboard device.
        /// </remarks>
        /// <unmanaged>RIDEV_APPKEYS</unmanaged>
        AppKeys = 0x00000400,

        /// <summary>
        ///   Enable the caller to receive input in the background only if the foreground application does
        ///   not process it (i.e. not registered for raw input).
        /// </summary>
        /// <unmanaged>RIDEV_EXINPUTSINK</unmanaged>
        ExclusiveInputSink = 0x00001000,

        /// <summary>
        ///   Enable the caller to receive <b>input device change</b> notifications for device arrival
        ///   and device removal.
        /// </summary>
        /// <unmanaged>RIDEV_DEVNOTIFY</unmanaged>
        DeviceNotify = 0x00002000
    }
}
