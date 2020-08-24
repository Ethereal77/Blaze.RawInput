// Copyright © 2020 Infinisis

using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

using static Blaze.Framework.RawInput.RawInput;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Internal RawInput message filtering.
    /// </summary>
    internal class RawInputMessageFilter : IMessageFilter
    {
        private const int WM_INPUT = 0x00FF;

        public virtual bool PreFilterMessage(ref Message message)
        {
            if (message.Msg == WM_INPUT)
                HandleMessage(message.HWnd, message.LParam);

            return false;
        }

        /// <summary>
        ///   Handles a RawInput message manually.
        /// </summary>
        /// <param name="hWnd">The handle of the window that received the RawInput message.</param>
        /// <param name="ptrRawInputMessage">A pointer to a RawInput message.</param>
        /// <remarks>
        ///   This method can be used directly when handling RawInput messages from non-WinForms application.
        /// </remarks>
        public static unsafe void HandleMessage(IntPtr hWnd, IntPtr ptrRawInputMessage)
        {
            uint dataLength = 0;
            GetRawInputData(ptrRawInputMessage, RawInputDataType.Input, IntPtr.Zero, ref dataLength, (uint) Unsafe.SizeOf<RawInputData.RawInputHeader>());
            if (dataLength == 0)
                return;

            byte* inputData = stackalloc byte[(int) dataLength];
            GetRawInputData(ptrRawInputMessage, RawInputDataType.Input, (IntPtr) inputData, ref dataLength, (uint) Unsafe.SizeOf<RawInputData.RawInputHeader>());
            RawInputData* rawInputPtr = (RawInputData*) inputData;
            var device = rawInputPtr->Header.Device;
            switch (rawInputPtr->Header.Type)
            {
                case DeviceType.Mouse:
                    {
                        if (MouseInput is null)
                            break;

                        var eventArgs = new MouseInputEventArgs(in *rawInputPtr);
                        MouseInput(device, hWnd, in eventArgs);
                        break;
                    }

                case DeviceType.Keyboard:
                    {
                        if (KeyboardInput is null)
                            break;

                        var eventArgs = new KeyboardInputEventArgs(in *rawInputPtr);
                        KeyboardInput(device, hWnd, in eventArgs);
                        break;
                    }

                case DeviceType.HumanInputDevice:
                    {
                        if (Input is null)
                            break;

                        var eventArgs = new HidInputEventArgs(in *rawInputPtr);
                        Input(device, hWnd, in eventArgs);
                        break;
                    }
            }
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

        #endregion
    }
}
