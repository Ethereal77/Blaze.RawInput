// Copyright © 2020 Infinisis

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Defines the keyboard input notifications or states received through RawInput.
    /// </summary>
    /// <unmanaged>RAW_KEY_STATE</unmanaged>
    public enum KeyState : uint
    {
        /// <summary>
        ///   A non-system key has been pressed in a window with the keyboard focus.
        /// </summary>
        /// <remarks>
        ///   A non-system key is a key that is pressed when the ALT key is not pressed.
        /// </remarks>
        /// <unmanaged>WM_KEYDOWN</unmanaged>
        KeyDown = 0x00000100,

        /// <summary>
        ///   A non-system key has been released in a window with the keyboard focus.
        /// </summary>
        /// <remarks>
        ///   A non-system key is a key that is pressed when the ALT key is not pressed.
        /// </remarks>
        /// <unmanaged>WM_KEYUP</unmanaged>
        KeyUp = 0x00000101,

        /// <summary>
        ///   A key has been pressed while pressing ALT or the F10 key has been pressed.
        /// </summary>
        /// <remarks>
        ///   This state also occurs when no window currently has the keyboard focus; in this
        ///   case, the state would be visible in the active window.
        /// </remarks>
        /// <unmanaged>WM_SYSKEYDOWN</unmanaged>
        SystemKeyDown = 0x00000104,

        /// <summary>
        ///   A key has been released that was pressed while the ALT kay was held down.
        /// </summary>
        /// <remarks>
        ///   This state also occurs when no window currently has the keyboard focus; in this
        ///   case, the state would be visible in the active window.
        /// </remarks>
        /// <unmanaged>WM_SYSKEYUP</unmanaged>
        SystemKeyUp = 0x00000105,

        /// <summary>
        ///   A key pressed notification sent by the IME (Input Method Editor) in a window with the keyboard focus..
        /// </summary>
        /// <unmanaged>WM_IME_KEYDOWN</unmanaged>
        ImeKeyDown = 0x00000290,

        /// <summary>
        ///   A key released notification sent by the IME (Input Method Editor) in a window with the keyboard focus..
        /// </summary>
        /// <unmanaged>WM_IME_KEYUP</unmanaged>
        ImeKeyUp = 0x00000291,

        /// <summary>
        ///   An application-specific registered hot key has been pressed.
        /// </summary>
        /// <unmanaged>WM_HOTKEY</unmanaged>
        HotKey = 0x00000312
    }
}
