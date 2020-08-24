// Copyright © 2020 Infinisis

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Defines the data types that a call to <see cref="RawInput.GetRawInputData"/>
    ///   can reference.
    /// </summary>
    /// <unmanaged>RAW_INPUT_DATA_TYPE</unmanaged>
    internal enum RawInputDataType : uint
    {
        /// <summary>
        ///   Get the raw data from the <see cref="RawInputData"/> structure.
        /// </summary>
        /// <unmanaged>RID_INPUT</unmanaged>
        Input = 0x10000003,

        /// <summary>
        ///   Get the header information from the <see cref="RawInputData"/> structure.
        /// </summary>
        /// <unmanaged>RID_HEADER</unmanaged>
        Header = 0x10000005,
    }
}
