// Copyright © 2020 Infinisis

using System;
using System.Windows.Forms;

using static Blaze.Framework.RawInput.RawInput;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Internal RawInput message filtering.
    /// </summary>
    internal class RawInputMessageFilter : IMessageFilter
    {
        private const int WM_INPUT               = 0x00FF;
        private const int WM_INPUT_DEVICE_CHANGE = 0x00FE;

        public virtual bool PreFilterMessage(ref Message message)
        {
            if (message.Msg == WM_INPUT)
                HandleInput(message.HWnd, message.LParam, message.WParam);

            else if (message.Msg == WM_INPUT_DEVICE_CHANGE)
                HandleDeviceChange(message.LParam, message.WParam);

            return false;
        }

        /// <summary>
        ///   Handles a RawInput message.
        /// </summary>
        /// <param name="hWnd">The handle of the window that received the RawInput message.</param>
        /// <param name="ptrRawInputMessage">A pointer to a RawInput message.</param>
        /// <param name="rawInputMode">
        ///   A value indicating whether the input message is from the regular message flow or
        ///   if the message is for a input sink. Use <see cref="GetRawInputCode"/> to know which one.
        /// </param>
        /// <remarks>
        ///   This method can be used directly when handling RawInput messages from non-WinForms application.
        /// </remarks>
        private static void HandleInput(IntPtr hWnd, IntPtr ptrRawInputMessage, IntPtr rawInputMode)
        {
            uint dataLength = GetRawInputDataSize(ptrRawInputMessage);
            if (dataLength == 0)
                return;

            Span<byte> inputData = stackalloc byte[(int) dataLength];
            ref RawInputData rawInput = ref GetRawInputData(ptrRawInputMessage, inputData);

            var device = rawInput.Header.Device;
            switch (rawInput.Header.Type)
            {
                case DeviceType.Mouse:
                    {
                        if (MouseInput is null)
                            break;

                        var eventArgs = new MouseInputEventArgs(in rawInput, rawInputMode);
                        MouseInput(device, hWnd, in eventArgs);
                        break;
                    }

                case DeviceType.Keyboard:
                    {
                        if (KeyboardInput is null)
                            break;

                        var eventArgs = new KeyboardInputEventArgs(in rawInput, rawInputMode);
                        KeyboardInput(device, hWnd, in eventArgs);
                        break;
                    }

                case DeviceType.HumanInputDevice:
                    {
                        if (Input is null)
                            break;

                        var eventArgs = new HidInputEventArgs(in rawInput, rawInputMode);
                        Input(device, hWnd, in eventArgs);
                        break;
                    }
            }
        }

        /// <summary>
        ///   Handles a RawInput device change message.
        /// </summary>
        /// <param name="deviceHandle">Handle to the device that has changed.</param>
        /// <param name="deviceChange">A value indicating the change.</param>
        private static void HandleDeviceChange(IntPtr deviceHandle, IntPtr deviceChange)
        {
            var change = (DeviceChange) deviceChange;

            Devices.NotifyDeviceChanged(deviceHandle, change);

            DeviceChanged?.Invoke(deviceHandle, change);
        }

        #region Events

        /// <summary>
        ///   Occurs when a keyboard input event is received.
        /// </summary>
        internal static event KeyboardInputEventHandler KeyboardInput;

        /// <summary>
        ///   Occurs when a mouse input event is received.
        /// </summary>
        internal static event MouseInputEventHandler MouseInput;

        /// <summary>
        ///   Occurs when a raw input event is received.
        /// </summary>
        internal static event RawInputEventHandler Input;

        /// <summary>
        ///   Occurs when a raw input device has been added to or removed from the system.
        /// </summary>
        internal static event DeviceChangedEventHandler DeviceChanged;

        #endregion
    }
}
