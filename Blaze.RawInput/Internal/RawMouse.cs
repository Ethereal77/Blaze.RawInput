// Copyright © 2020 Infinisis

using System.Runtime.InteropServices;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Contains information about the state of the mouse.
    /// </summary>
    /// <unmanaged>RAWMOUSE</unmanaged>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawMouse
    {
        /// <summary>
        ///   The mouse state. It can be any combination of values of <see cref="MouseMode"/>.
        /// </summary>
        public MouseMode Flags;

        /// <summary>
        ///   The state of the mouse buttons.
        /// </summary>
        public RawMouseButtonsData ButtonsData;

        /// <summary>
        ///   The raw state of the mouse buttons.
        /// </summary>
        /// <remarks>The Win32 subsystem does not use this member.</remarks>
        public int RawButtons;

        /// <summary>
        ///   The motion in the X direction. This is signed relative motion or
        ///   absolute motion, depending on the value of <see cref="Flags"/>.
        /// </summary>
        /// <remarks>
        ///   In <see cref="MouseMode.MoveRelative"/> mode, this value specifies movement relative to the
        ///   previous mouse event (the previous reported position). Positive values mean the mouse moved
        ///   right; negative values mean the mouse moved left.
        ///   <para/>
        ///   In <see cref="MouseMode.MoveAbsolute"/> mode, this value contains a normalized absolute
        ///   coordinate between 0 and 65535. Coordinate (0,0) maps onto the upper-left corner of the display
        ///   surface; Coordinate (65535,65535) maps onto the lower-right corner. In a multi-monitor system,
        ///   the coordinates map to the primary monitor.
        ///   <para/>
        ///   If <see cref="MouseMode.VirtualDesktop"/> is specified in addition to <see cref="MouseMode.MoveAbsolute"/>,
        ///   the coordinates map to the entire virtual desktop.
        /// </remarks>
        public int LastX;

        /// <summary>
        /// The motion in the Y direction. This is signed relative motion or absolute motion,
        /// depending on the value of <see cref="Flags"/>.
        /// </summary>
        /// <remarks>
        ///   In <see cref="MouseMode.MoveRelative"/> mode, this value specifies movement relative to the
        ///   previous mouse event (the previous reported position). Positive values mean the mouse moved
        ///   down; negative values mean the mouse moved up.
        ///   <para/>
        ///   In <see cref="MouseMode.MoveAbsolute"/> mode, this value contains a normalized absolute
        ///   coordinate between 0 and 65535. Coordinate (0,0) maps onto the upper-left corner of the display
        ///   surface; Coordinate (65535,65535) maps onto the lower-right corner. In a multi-monitor system,
        ///   the coordinates map to the primary monitor.
        ///   <para/>
        ///   If <see cref="MouseMode.VirtualDesktop"/> is specified in addition to <see cref="MouseMode.MoveAbsolute"/>,
        ///   the coordinates map to the entire virtual desktop.
        /// </remarks>
        public int LastY;

        /// <summary>
        ///   The device-specific additional information for the event.
        /// </summary>
        public int ExtraInformation;

        [StructLayout(LayoutKind.Explicit)]
        public struct RawMouseButtonsData
        {
            [FieldOffset(0)]
            public int Buttons;

            /// <summary>
            ///   The transition state of the mouse buttons.
            /// </summary>
            [FieldOffset(0)]
            public MouseButtonFlags ButtonFlags;

            /// <summary>
            ///   If the mouse wheel is moved, this specifies the distance the wheel is rotated.
            /// </summary>
            [FieldOffset(2)]
            public short ButtonData;
        }
    }
}
