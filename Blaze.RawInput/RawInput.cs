// Copyright © 2020 Infinisis

using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

using Blaze.Interop.Win32;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Supplies functions from the Raw Input API which provides a stable and robust way for applications
    ///   to accept raw input from any HID (Human Input Device), including the keyboard and mouse.
    /// </summary>
    public static partial class RawInput
    {
        #region Device Enumeration and Information

        private static Lazy<DeviceInfoCollection> g_Devices =
            new Lazy<DeviceInfoCollection>(() => new DeviceInfoCollection(), isThreadSafe: true);

        /// <summary>
        ///   Gets a collection of the available RawInput devices attached to the system.
        /// </summary>
        public static DeviceInfoCollection Devices => g_Devices.Value;

        #endregion

        #region Device Registration

        private static RawInputMessageFilter rawInputMessageFilter;

        /// <summary>
        ///   Registers the devices that supply the raw input data.
        /// </summary>
        /// <param name="usagePage">The usage page.</param>
        /// <param name="usageId">The usage id.</param>
        /// <param name="flags">The flags.</param>
        public static void RegisterDevice(UsagePage usagePage, UsageId usageId, DeviceFlags flags) =>
            RegisterDevice(usagePage, usageId, flags, targetHwnd: IntPtr.Zero);

        /// <summary>
        ///   Registers the devices that supply the raw input data.
        /// </summary>
        /// <param name="usagePage">The usage page.</param>
        /// <param name="usageId">The usage id.</param>
        /// <param name="flags">A combination of flags specifying how to interpret the values of <paramref name="usageId"/> and <paramref name="usagePage"/>.</param>
        /// <param name="targetHwnd">A handle to the target window.</param>
        /// <param name="options">The options for registering the device.</param>
        public static void RegisterDevice(UsagePage usagePage, UsageId usageId, DeviceFlags flags, IntPtr targetHwnd, RegisterDeviceOptions options = RegisterDeviceOptions.Default)
        {
            RawInputDevice rawInputDevice;
            rawInputDevice.UsagePage = usagePage;
            rawInputDevice.UsageId = usageId;
            rawInputDevice.Flags = flags;
            rawInputDevice.Target = targetHwnd;

            RegisterRawInputDevice(rawInputDevice);

            if (options == RegisterDeviceOptions.NoFiltering || rawInputMessageFilter != null)
                return;

            rawInputMessageFilter = new RawInputMessageFilter();
            if (options == RegisterDeviceOptions.Default)
                Application.AddMessageFilter(rawInputMessageFilter);
            else
                MessageFilterHook.AddMessageFilter(targetHwnd, rawInputMessageFilter);
        }

        #endregion

        #region Get Buffered RawInput Data

        /// <summary>
        ///   Processes the raw input messages received by the application.
        /// </summary>
        /// <remarks>
        ///   Call this function to process the queued raw input readings of the registered devices.
        ///   <para/>
        ///   Using this function, the raw input data is buffered and processed in bulk. In contrast, you can use message processing
        ///   by specifying <see cref="RegisterDeviceOptions.Default"/> or <see cref="RegisterDeviceOptions.CustomFiltering"/> when
        ///   calling <see cref="RegisterDevice(UsagePage, UsageId, DeviceFlags, IntPtr, RegisterDeviceOptions)"/> to enable a filter
        ///   that processes the messages received by the application.
        ///   <para/>
        ///   This way is more convenient for reading raw input data from devices generating a large amount of data.
        /// </remarks>
        public static void ProcessMessages()
        {
            // Get the approx. size of a RawInput struct (including extra data for HIDs)
            var bufferSize = (int) GetRawInputBufferSize();
            if (bufferSize == 0)
                return;

            Span<byte> buffer = stackalloc byte[bufferSize * 16];

            // While we keep reading messages, fill the buffer and process them one by one
            while (true)
            {
                var eventCount = GetRawInputBuffer(buffer);
                if (eventCount == 0)
                    break;

                ref RawInputData rawInput = ref Unsafe.As<byte, RawInputData>(ref buffer[0]);

                for (int index = 0; index < eventCount; index++)
                {
                    HandleRawInput(in rawInput);

                    rawInput = ref NextRawInputBlock(in rawInput);

                    //DefRawInputProc(in rawInput, eventCount, Unsafe.SizeOf(RawInputHeader));
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        ///   Processes a <see cref="RawInputData"/> structure and dispatches the corresponding events.
        /// </summary>
        /// <param name="rawInput">Reference to the <see cref="RawInputData"/> structure.</param>
        /// <param name="windowHandle">Handle of the window receiving the message.</param>
        internal static void HandleRawInput(in RawInputData rawInput, IntPtr windowHandle = default)
        {
            switch (rawInput.Header.Type)
            {
                case DeviceType.Mouse:
                    RaiseMouseInput(rawInput.Header.Device, windowHandle, in rawInput);
                    break;

                case DeviceType.Keyboard:
                    RaiseKeyboardInput(rawInput.Header.Device, windowHandle, in rawInput);
                    break;

                case DeviceType.HumanInputDevice:
                    RaiseInput(rawInput.Header.Device, windowHandle, in rawInput);
                    break;
            }
        }

        /// <summary>
        ///   Occurs when a keyboard input event is received.
        /// </summary>
        public static event KeyboardInputEventHandler KeyboardInput;

        /// <summary>
        ///   Raises the <see cref="KeyboardInput"/> event.
        /// </summary>
        internal static void RaiseKeyboardInput(IntPtr device, IntPtr hWnd, in RawInputData rawInput)
        {
            if (KeyboardInput is null)
                return;

            var eventArgs = new KeyboardInputEventArgs(in rawInput);
            KeyboardInput(device, hWnd, in eventArgs);
        }

        /// <summary>
        ///   Occurs when a mouse input event is received.
        /// </summary>
        public static event MouseInputEventHandler MouseInput;

        /// <summary>
        ///   Raises the <see cref="MouseInput"/> event.
        /// </summary>
        internal static void RaiseMouseInput(IntPtr device, IntPtr hWnd, in RawInputData rawInput)
        {
            if (MouseInput is null)
                return;

            var eventArgs = new MouseInputEventArgs(in rawInput);
            MouseInput(device, hWnd, in eventArgs);
        }

        /// <summary>
        ///   Occurs when a raw input event is received.
        /// </summary>
        /// <remarks>
        ///   This event is raised when an message is received from a HID (Human Input Device)
        ///   of any type other than a keyboard or a mouse.
        ///   <para/>
        ///   For receiving notifications of mouse events, use <see cref="MouseInput"/>. For receiving
        ///   notifications of keyboard events, use <see cref="KeyboardInput"/>.
        /// </remarks>
        public static event RawInputEventHandler Input;

        /// <summary>
        ///   Raises the <see cref="Input"/> event.
        /// </summary>
        internal static void RaiseInput(IntPtr device, IntPtr hWnd, in RawInputData rawInput)
        {
            if (Input is null)
                return;

            var eventArgs = new HidInputEventArgs(in rawInput);
            Input(device, hWnd, in eventArgs);
        }

        /// <summary>
        ///   Occurs when a raw input device has been added to or removed from the system.
        /// </summary>
        /// <remarks>
        ///   This event provides the handle of the device that was attached or removed from the system.
        ///   If the change is an arrival of a new device attached to the system, you can get information
        ///   about the device from <see cref="Devices"/>.
        /// </remarks>
        public static event DeviceChangedEventHandler DeviceChanged;

        /// <summary>
        ///   Raises the <see cref="DeviceChanged"/> event.
        /// </summary>
        internal static void RaiseDeviceChanged(IntPtr deviceHandle, DeviceChange deviceChange)
        {
            Devices.NotifyDeviceChanged(deviceHandle, deviceChange);

            DeviceChanged?.Invoke(deviceHandle, deviceChange);
        }

        #endregion
    }
}
