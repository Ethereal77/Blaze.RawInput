// Copyright © 2020 Infinisis

using System.Runtime.InteropServices;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Describes the format of the raw input from a Human Input Device (HID).
    /// </summary>
    /// <remarks>
    ///   Each raw input event (<c>WM_INPUT</c>) can indicate several inputs, but all of the inputs come from
    ///   the same HID.
    ///   The size of the <see cref="RawData"/> array is <b><see cref="SizeHid"/> * <see cref="Count"/></b>.
    /// </remarks>
    /// <unmanaged>RAWHID</unmanaged>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct RawHid
    {
        /// <summary>
        ///   The size of each HID input in <see cref="RawData"/>, in bytes.
        /// </summary>
        /// <unmanaged>dwSizeHid</unmanaged>
        public readonly uint SizeHid;

        /// <summary>
        ///   The number of HID inputs in <see cref="RawData"/>.
        /// </summary>
        /// <unmanaged>dwCount</unmanaged>
        public readonly uint Count;

        /// <summary>
        ///   The raw input data, as an array of bytes.
        /// </summary>
        /// <unmanaged>bRawData</unmanaged>
        public readonly byte RawData;
    }
}
