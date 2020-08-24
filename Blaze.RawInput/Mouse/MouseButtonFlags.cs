// Copyright © 2020 Infinisis

using System;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Defines the transition state of the mouse buttons.
    /// </summary>
    [Flags]
    public enum MouseButtonFlags : ushort
    {
        /// <unmanaged>None</unmanaged>
        None = 0,

        /// <summary>
        ///   Left button changed to down.
        /// </summary>
        /// <unmanaged>RI_MOUSE_LEFT_BUTTON_DOWN</unmanaged>
        LeftButtonDown = 1,

        /// <summary>
        ///   Left button changed to up.
        /// </summary>
        /// <unmanaged>RI_MOUSE_LEFT_BUTTON_UP</unmanaged>
        LeftButtonUp = 2,

        /// <summary>
        ///   Right button changed to down.
        /// </summary>
        /// <unmanaged>RI_MOUSE_RIGHT_BUTTON_DOWN</unmanaged>
        RightButtonDown = 4,

        /// <summary>
        ///   Right button changed to up.
        /// </summary>
        /// <unmanaged>RI_MOUSE_RIGHT_BUTTON_UP</unmanaged>
        RightButtonUp = 8,

        /// <summary>
        ///   Middle button changed to down.
        /// </summary>
        /// <unmanaged>RI_MOUSE_MIDDLE_BUTTON_DOWN</unmanaged>
        MiddleButtonDown = 0x00000010,

        /// <summary>
        ///   Middle button changed to up.
        /// </summary>
        /// <unmanaged>RI_MOUSE_MIDDLE_BUTTON_UP</unmanaged>
        MiddleButtonUp = 0x00000020,

        /// <summary>
        ///   Button 1 (left button) changed to down.
        /// </summary>
        /// <unmanaged>RI_MOUSE_BUTTON_1_DOWN</unmanaged>
        Button1Down = LeftButtonDown,

        /// <summary>
        ///   Button 1 (left button) changed to up.
        /// </summary>
        /// <unmanaged>RI_MOUSE_BUTTON_1_UP</unmanaged>
        Button1Up = LeftButtonUp,

        /// <summary>
        ///   Button 2 (right button) changed to down.
        /// </summary>
        /// <unmanaged>RI_MOUSE_BUTTON_2_DOWN</unmanaged>
        Button2Down = RightButtonDown,

        /// <summary>
        ///   Button 2 (right button) changed to up.
        /// </summary>
        /// <unmanaged>RI_MOUSE_BUTTON_2_UP</unmanaged>
        Button2Up = RightButtonUp,

        /// <summary>
        ///   Button 3 (middle button) changed to down.
        /// </summary>
        /// <unmanaged>RI_MOUSE_BUTTON_3_DOWN</unmanaged>
        Button3Down = MiddleButtonDown,

        /// <summary>
        ///   Button 3 (middle button) changed to up.
        /// </summary>
        /// <unmanaged>RI_MOUSE_BUTTON_3_UP</unmanaged>
        Button3Up = MiddleButtonUp,

        /// <summary>
        ///   Extra button 1 changed to down.
        /// </summary>
        /// <unmanaged>RI_MOUSE_BUTTON_4_DOWN</unmanaged>
        Button4Down = 0x00000040,

        /// <summary>
        ///   Extra button 1 changed to up.
        /// </summary>
        /// <unmanaged>RI_MOUSE_BUTTON_4_UP</unmanaged>
        Button4Up = 0x00000080,

        /// <summary>
        ///   Extra button 2 changed to down.
        /// </summary>
        /// <unmanaged>RI_MOUSE_BUTTON_5_DOWN</unmanaged>
        Button5Down = 0x00000100,

        /// <summary>
        ///   Extra button 1 changed to up.
        /// </summary>
        /// <unmanaged>RI_MOUSE_BUTTON_5_UP</unmanaged>
        Button5Up = 0x00000200,

        /// <summary>
        ///   Raw input comes from a mouse wheel.
        /// </summary>
        /// <remarks>
        ///   The wheel delta is stored in <see cref="RawMouse.RawMouseButtonsData.ButtonData"/>.
        ///   <para/>
        ///   A positive value indicates that the wheel was rotated forward, away from the user;
        ///   a negative value indicates that the wheel was rotated backward, toward the user.
        /// </remarks>
        /// <unmanaged>RI_MOUSE_WHEEL</unmanaged>
        MouseWheel = 0x00000400,

        /// <summary>
        ///   Raw input comes from a horizontal mouse wheel.
        /// </summary>
        /// <remarks>
        ///   The wheel delta is stored in <see cref="RawMouse.RawMouseButtonsData.ButtonData"/>.
        ///   <para/>
        ///   A positive value indicates that the wheel was rotated to the right;
        ///   a negative value indicates that the wheel was rotated to the left.
        /// </remarks>
        /// <unmanaged>RI_MOUSE_HWHEEL</unmanaged>
        HorizontalMouseWheel = 0x00000800
    }
}
