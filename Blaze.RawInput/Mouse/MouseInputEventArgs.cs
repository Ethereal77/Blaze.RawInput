// Copyright © 2020 Infinisis

using System;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Represents the method that handles the <see cref="RawInput.MouseInput"/> event for receiving
    ///   raw input from a mouse.
    /// </summary>
    /// <param name="device">Source raw input device.</param>
    /// <param name="windowHandle">Handle of the window that has received the input.</param>
    /// <param name="e">A <see cref="MouseInputEventArgs"/> structure containing information about the received input.</param>
    public delegate void MouseInputEventHandler(IntPtr device, IntPtr windowHandle, in MouseInputEventArgs e);

    /// <summary>
    ///   Describes the arguments for an raw input event from a mouse.
    /// </summary>
    public readonly ref struct MouseInputEventArgs
    {
        private readonly RawMouse mouseInput;

        /// <summary>
        ///   Gets the mode of the mouse.
        /// </summary>
        public readonly MouseMode Mode => mouseInput.Flags;

        /// <summary>
        ///   Gets the button flags.
        /// </summary>
        public readonly MouseButtonFlags ButtonFlags => mouseInput.ButtonsData.ButtonFlags;

        /// <summary>
        ///   Gets the extra information.
        /// </summary>
        public readonly int ExtraInformation => mouseInput.ExtraInformation;

        /// <summary>
        ///   Gets the raw buttons.
        /// </summary>
        public readonly int Buttons => mouseInput.RawButtons;

        /// <summary>
        ///   Gets the wheel delta.
        /// </summary>
        public readonly int WheelDelta => mouseInput.ButtonsData.ButtonData;

        /// <summary>
        ///   Gets the X.
        /// </summary>
        public readonly int X => mouseInput.LastX;

        /// <summary>
        ///   Gets the Y.
        /// </summary>
        public readonly int Y => mouseInput.LastY;


        /// <summary>
        ///   Initializes a new instance of the <see cref="MouseInputEventArgs" /> class.
        /// </summary>
        /// <param name="rawInput">The raw input data.</param>
        internal MouseInputEventArgs(in RawInputData rawInput)
        {
            mouseInput = rawInput.Data.Mouse;
        }
    }
}
