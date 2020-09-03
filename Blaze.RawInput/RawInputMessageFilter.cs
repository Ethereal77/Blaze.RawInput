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
                HandleInput(message.HWnd, message.LParam);

            else if (message.Msg == WM_INPUT_DEVICE_CHANGE)
                HandleDeviceChange(message.LParam, message.WParam);

            return false;
        }

        /// <summary>
        ///   Handles a RawInput message.
        /// </summary>
        /// <param name="windowHandle">The handle of the window that received the RawInput message.</param>
        /// <param name="ptrRawInputMessage">A pointer to a RawInput message.</param>
        /// <remarks>
        ///   This method can be used directly when handling RawInput messages from non-WinForms application.
        /// </remarks>
        private static void HandleInput(IntPtr windowHandle, IntPtr ptrRawInputMessage)
        {
            uint dataLength = GetRawInputDataSize(ptrRawInputMessage);
            if (dataLength == 0)
                return;

            Span<byte> inputData = stackalloc byte[(int) dataLength];
            ref RawInputData rawInput = ref GetRawInputData(ptrRawInputMessage, inputData);

            HandleRawInput(in rawInput, windowHandle);
        }

        /// <summary>
        ///   Handles a RawInput device change message.
        /// </summary>
        /// <param name="deviceHandle">Handle to the device that has changed.</param>
        /// <param name="deviceChange">A value indicating the change.</param>
        private static void HandleDeviceChange(IntPtr deviceHandle, IntPtr deviceChange)
        {
            RaiseDeviceChanged(deviceHandle, (DeviceChange) deviceChange);
        }
    }
}
