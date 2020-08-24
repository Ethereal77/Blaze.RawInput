// Copyright © 2020 Infinisis

using System;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Defines the mouse mode flags.
    /// </summary>
    [Flags]
    public enum MouseMode : ushort
    {
        /// <summary>
        ///   Mouse movement data is relative to the last mouse position.
        /// </summary>
        /// <unmanaged>MOUSE_MOVE_RELATIVE</unmanaged>
        MoveRelative = 0,

        /// <summary>
        ///   Mouse movement data is based on absolute position.
        /// </summary>
        /// <unmanaged>MOUSE_MOVE_ABSOLUTE</unmanaged>
        MoveAbsolute = 1,

        /// <summary>
        ///   Mouse coordinates are mapped to the virtual desktop (for a multiple monitor system).
        /// </summary>
        /// <unmanaged>MOUSE_VIRTUAL_DESKTOP</unmanaged>
        VirtualDesktop = 2,

        /// <summary>
        ///   Mouse attributes changed. The application needs to query the mouse attributes.
        /// </summary>
        /// <unmanaged>MOUSE_ATTRIBUTES_CHANGED</unmanaged>
        AttributesChanged = 4,

        /// <summary>
        ///   The mouse movement event was not coalesced.
        /// </summary>
        /// <unmanaged>MOUSE_MOVE_NOCOALESCE</unmanaged>
        MoveNoCoalesce = 8
    }
}
