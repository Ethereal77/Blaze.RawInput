// Copyright © 2020 Infinisis

using System;
using System.Windows.Forms;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Represents the method that handles the <see cref="RawInput.KeyboardInput"/> event for receiving
    ///   raw input from a keyboard.
    /// </summary>
    /// <param name="device">Source raw input device.</param>
    /// <param name="windowHandle">Handle of the window that has received the input.</param>
    /// <param name="e">A <see cref="KeyboardInputEventArgs"/> structure containing information about the received input.</param>
    public delegate void KeyboardInputEventHandler(IntPtr device, IntPtr windowHandle, in KeyboardInputEventArgs e);

    /// <summary>
    ///   Describes the arguments for an raw input event from a keyboard.
    /// </summary>
    public readonly ref struct KeyboardInputEventArgs
    {
        private readonly RawKeyboard keyboardInput;

        /// <summary>
        ///   Gets the key.
        /// </summary>
        public readonly Keys Key => (Keys) keyboardInput.VirtualKey;

        /// <summary>
        ///   Gets the make code.
        /// </summary>
        public readonly int MakeCode => keyboardInput.MakeCode;

        /// <summary>
        ///   Gets the scan code flags.
        /// </summary>
        public readonly ScanCodeFlags ScanCodeFlags => keyboardInput.Flags;

        /// <summary>
        ///   Gets the state.
        /// </summary>
        public readonly KeyState State => keyboardInput.Message;

        /// <summary>
        ///   Gets the extra information.
        /// </summary>
        public readonly uint ExtraInformation => keyboardInput.ExtraInformation;

        /// <summary>
        ///   Gets the input mode of the event.
        /// </summary>
        public readonly InputMode InputMode { get; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="KeyboardInputEventArgs" /> structure.
        /// </summary>
        /// <param name="rawInput">The raw input data.</param>
        internal KeyboardInputEventArgs(in RawInputData rawInput)
        {
            keyboardInput = rawInput.Data.Keyboard;

            InputMode = RawInput.GetRawInputCode(rawInput.Header.WParam);
        }
    }
}
