// Copyright © 2020 Infinisis

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Represents the method that handles the <see cref="RawInput.Input"/> event for receiving
    ///   raw input from HIDs.
    /// </summary>
    /// <param name="device">Source raw input device.</param>
    /// <param name="windowHandle">Handle of the window that has received the input.</param>
    /// <param name="e">A <see cref="HidInputEventArgs"/> structure containing information about the received input.</param>
    public delegate void RawInputEventHandler(IntPtr device, IntPtr windowHandle, in HidInputEventArgs e);

    /// <summary>
    ///   Describes the arguments for an raw input event from a Human Input Device (HID).
    /// </summary>
    public readonly ref struct HidInputEventArgs
    {
        /// <summary>
        ///   Number of HID structures in the <see cref="RawData"/>.
        /// </summary>
        private readonly uint Count;

        /// <summary>
        ///   Size of the HID structures in the <see cref="RawData"/>, in bytes.
        /// </summary>
        public readonly uint DataSize;

        /// <summary>
        ///   Raw input data.
        /// </summary>
        public readonly ReadOnlySpan<byte> RawData;

        /// <summary>
        ///   Gets the input mode of the event.
        /// </summary>
        public readonly InputMode InputMode => RawInput.GetRawInputCode(inputMode);
        private readonly IntPtr inputMode;


        /// <summary>
        ///   Initializes a new instance of the <see cref="HidInputEventArgs" /> structure.
        /// </summary>
        /// <param name="rawInput">The raw input data.</param>
        /// <param name="rawInputMode">The input mode as specified in <see cref="Message.WParam"/></param>
        internal HidInputEventArgs(in RawInputData rawInput, IntPtr rawInputMode)
        {
            Count = rawInput.Data.Hid.Count;
            DataSize = rawInput.Data.Hid.SizeHid;

            var rawDataLength = (int) (Count * DataSize);
            Debug.Assert(rawDataLength > 0);

            RawData = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in rawInput.Data.Hid.RawData), rawDataLength);

            inputMode = rawInputMode;
        }
    }
}
