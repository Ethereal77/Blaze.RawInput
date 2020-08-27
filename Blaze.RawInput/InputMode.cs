// Copyright © 2020 Infinisis

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Defines the the type of raw input received in an event.
    /// </summary>
    public enum InputMode : uint
    {
        /// <summary>
        ///   The input event forms part of the regular message flow.
        /// </summary>
        Input = 0,

        /// <summary>
        ///   The input event is received because the device is registered as an input sink
        ///   with the flag <see cref="DeviceFlags.InputSink"/> or <see cref="DeviceFlags.ExclusiveInputSink"/>.
        /// </summary>
        InputSink = 1
    }
}
