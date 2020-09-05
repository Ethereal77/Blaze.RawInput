// Copyright © 2020 Infinisis

using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

using Blaze.Interop.Win32;

using static Blaze.Framework.RawInput.ThrowHelper;

namespace Blaze.Framework.RawInput
{
    /// <summary>
    ///   Supplies functions from the Raw Input API which provides a stable and robust way for applications
    ///   to accept raw input from any HID (Human Input Device), including the keyboard and mouse.
    /// </summary>
    public static partial class RawInput
    {
        #region Device Enumeration and Information

        private static readonly Lazy<DeviceInfoCollection> g_Devices =
            new Lazy<DeviceInfoCollection>(() => new DeviceInfoCollection(), isThreadSafe: true);

        private static readonly Lazy<RegisteredDeviceCollection> g_RegisteredDevices =
            new Lazy<RegisteredDeviceCollection>(() => new RegisteredDeviceCollection(), isThreadSafe: true);

        /// <summary>
        ///   Gets a collection of the available RawInput devices attached to the system.
        /// </summary>
        public static DeviceInfoCollection Devices => g_Devices.Value;

        /// <summary>
        ///   Gets a collection of the RawInput devices registered by the application.
        /// </summary>
        public static RegisteredDeviceCollection RegisteredDevices => g_RegisteredDevices.Value;


        /// <summary>
        ///   Updates the internal list of registered devices when a device is registered or removed.
        /// </summary>
        private static void UpdateRegisteredDeviceList()
        {
            if (g_RegisteredDevices.IsValueCreated)
                g_RegisteredDevices.Value.Update();
        }

        #endregion

        #region Device Registration

        /// <summary>
        ///   Registers a device that supplies the raw input data.
        /// </summary>
        /// <param name="usagePage">The usage page.</param>
        /// <param name="usageId">The usage id.</param>
        /// <param name="flags">A combination of flags specifying how to interpret the values of <paramref name="usageId"/> and <paramref name="usagePage"/>.</param>
        /// <param name="windowHandle">
        ///   A handle to the target window. Specify <see cref="IntPtr.Zero"/> to not associate the device with a window. Instead the
        ///   input data will follow the input focus. This is useful when specifying the flag <see cref="DeviceFlags.InputSink"/> or
        ///   <see cref="DeviceFlags.ExclusiveInputSink"/>. The default value is <see cref="IntPtr.Zero"/>.
        /// </param>
        /// <remarks>
        ///   To receive <c>WM_INPUT</c> messages (through the events <see cref="MouseInput"/>, <see cref="KeyboardInput"/>, and
        ///   <see cref="Input"/>), an application must first register the raw input devices using <see cref="RegisterRawInputDevice"/>.
        ///   By default, an application does not receive raw input.
        ///   <para/>
        ///   To receive <c>WM_INPUT_DEVICE_CHANGE</c> messages (through the <see cref="DeviceChanged"/> event), an application must
        ///   specify <see cref="DeviceFlags.DeviceNotify"/> for each device class that is specified by the <see cref="UsagePage"/>
        ///   and <see cref="UsageId"/> of the  <see cref="RawInputDevice" /> structure. By default, an application does not receive
        ///   <c>WM_INPUT_DEVICE_CHANGE</c> notifications for raw input device arrival and removal.
        ///   <para/>
        ///   To unregister a device, specify <see cref="DeviceFlags.Remove"/>. This tells the operating system to stop reading
        ///   from a device that matches the top level collection (specified through <paramref name="usagePage"/> and
        ///   <paramref name="usageId"/>). Alternatively, use the <see cref="UnregisterDevice"/> method.
        ///   <para/>
        ///   If the <see cref="DeviceFlags.Remove"/> flag is set and <paramref name="windowHandle"/> is not set to
        ///   <see cref="IntPtr.Zero"/>, then parameter validation will fail.
        /// </remarks>
        public static void RegisterDevice(UsagePage usagePage, UsageId usageId, DeviceFlags flags, IntPtr windowHandle = default)
        {
            RawInputDevice rawInputDevice;
            rawInputDevice.UsagePage = usagePage;
            rawInputDevice.UsageId = usageId;
            rawInputDevice.Flags = flags;
            rawInputDevice.Target = windowHandle;

            if(!RegisterRawInputDevice(rawInputDevice))
                CheckLastResult();

            UpdateRegisteredDeviceList();
        }

        /// <summary>
        ///   Unregisters a device that supplies the raw input data.
        /// </summary>
        /// <param name="usagePage">The usage page.</param>
        /// <param name="usageId">The usage id.</param>
        /// <remarks>
        ///   This function tells the operating system to stop reading from a device that matches the top level collection
        ///   (specified through <paramref name="usagePage"/> and <paramref name="usageId"/>).
        /// </remarks>
        public static void UnregisterDevice(UsagePage usagePage, UsageId usageId) =>
            RegisterDevice(usagePage, usageId, DeviceFlags.Remove);

        #endregion

        #region Get RawInput Data

        private static IntPtr targetWindowHandle;
        private static RawInputMessageFilter rawInputMessageFilter;

        /// <summary>
        ///   Starts processing the raw input messages received by the application.
        /// </summary>
        /// <param name="windowHandle">Handle to the target window that will receive the messages.</param>
        /// <param name="options">The options for the processing and filtering of messages.</param>
        /// <remarks>
        ///   Call this function to start filtering and processing raw input messages through the message pump of a target window.
        ///   <para/>
        ///   Using this function, the raw input data is processed one message at a time. In contrast, you can use buffered processing
        ///   by calling <see cref="ProcessMessages"/> to process the queued raw input messages in bulk.
        ///   <para/>
        ///   This function installs a <see cref="IMessageFilter"/> that intercepts and processes the raw input messages received by
        ///   the application. See <see cref="MessageProcessingOptions"/> enumeration for more information.
        /// </remarks>
        public static void StartProcessingMessages(IntPtr windowHandle = default, MessageProcessingOptions options = MessageProcessingOptions.Default)
        {
            if (rawInputMessageFilter != null)
                return;

            targetWindowHandle = windowHandle;
            rawInputMessageFilter = new RawInputMessageFilter();

            if (options == MessageProcessingOptions.Default)
                Application.AddMessageFilter(rawInputMessageFilter);
            else
                MessageFilterHook.AddMessageFilter(windowHandle, rawInputMessageFilter);
        }

        /// <summary>
        ///   Stops processing the raw input messages received by the application.
        /// </summary>
        /// <remarks>
        ///   Call this function to stop filtering and processing raw input messages through the message pump of the target window as
        ///   previously specified by a call to <see cref="StartProcessingMessages"/>.
        ///   <para/>
        ///   This function removes any <see cref="IMessageFilter"/> previosly installed.
        /// </remarks>
        public static void StopProcessingMessages()
        {
            if (rawInputMessageFilter is null)
                return;

            Application.RemoveMessageFilter(rawInputMessageFilter);
            MessageFilterHook.RemoveMessageFilter(targetWindowHandle, rawInputMessageFilter);

            rawInputMessageFilter = null;
            targetWindowHandle = default;
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
        ///   by calling <see cref="StartProcessingMessages"/> to enable a filter that processes the messages received by the
        ///   application's message pump. You can stop this with <see cref="StopProcessingMessages"/>.
        ///   <para/>
        ///   This way is more convenient for reading raw input data from devices generating a large amount of data.
        /// </remarks>
        public static void ProcessMessages()
        {
            // Get the approx. size of a RawInput struct (including extra data for HIDs)
            var bufferSize = GetRawInputBufferSize();
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
