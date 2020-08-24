// Copyright © 2020 Infinisis

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

        private static DeviceInfo[] g_Devices;

        /// <summary>
        ///   Gets a collection of the available RawInput devices attached to the system.
        /// </summary>
        public static IEnumerable<DeviceInfo> Devices => g_Devices ??= GetDeviceInfos();

        /// <summary>
        ///   Gets information about the available raw input devices attached to the system.
        /// </summary>
        /// <returns>A array of <see cref="DeviceInfo"/> with the available input devices.</returns>
        private static unsafe DeviceInfo[] GetDeviceInfos()
        {
            uint numDevices = 0;
            GetRawInputDeviceList(null, ref numDevices, (uint) Unsafe.SizeOf<RawInputDeviceList>());
            if (numDevices == 0)
                return null;

            var rawInputDevices = new RawInputDeviceList[numDevices];
            GetRawInputDeviceList(rawInputDevices, ref numDevices, (uint) Unsafe.SizeOf<RawInputDeviceList>());
            var deviceInfos = new DeviceInfo[numDevices];
            for (int index = 0; index < numDevices; ++index)
            {
                IntPtr deviceHandle = rawInputDevices[index].Device;

                // Get the name of the device
                uint nameLength = 0;
                GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoType.DeviceName, IntPtr.Zero, ref nameLength);
                char* nameChars = stackalloc char[(int)nameLength];
                GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoType.DeviceName, (IntPtr) nameChars, ref nameLength);

                int indexCh = 0;
                do { } while (indexCh <= nameLength && nameChars[indexCh++] != 0);
                string deviceName = new string(nameChars, 0, indexCh == 0 ? 0 : indexCh - 1);

                // Get device info
                uint infoLength = 0;
                GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoType.DeviceInfo, IntPtr.Zero, ref infoLength);
                byte* infoBytes = stackalloc byte[(int)infoLength];
                GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoType.DeviceInfo, (IntPtr) infoBytes, ref infoLength);

                deviceInfos[index] = DeviceInfo.Create(ref *(RawDeviceInformation*) infoBytes, deviceName, deviceHandle);
            }

            return deviceInfos;
        }

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
            Span<RawInputDevice> rawInputDevice = stackalloc RawInputDevice[1];
            rawInputDevice[0].UsagePage = usagePage;
            rawInputDevice[0].UsageId = usageId;
            rawInputDevice[0].Flags = flags;
            rawInputDevice[0].Target = targetHwnd;

            RegisterRawInputDevices(rawInputDevice);

            if (options == RegisterDeviceOptions.NoFiltering || rawInputMessageFilter != null)
                return;

            rawInputMessageFilter = new RawInputMessageFilter();
            if (options == RegisterDeviceOptions.Default)
                Application.AddMessageFilter(rawInputMessageFilter);
            else
                MessageFilterHook.AddMessageFilter(targetHwnd, rawInputMessageFilter);
        }

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when a keyboard input event is received.
        /// </summary>
        public static event KeyboardInputEventHandler KeyboardInput
        {
            add { RawInputMessageFilter.KeyboardInput += value; }
            remove { RawInputMessageFilter.KeyboardInput -= value; }
        }

        /// <summary>
        ///   Occurs when a mouse input event is received.
        /// </summary>
        public static event MouseInputEventHandler MouseInput
        {
            add { RawInputMessageFilter.MouseInput += value; }
            remove { RawInputMessageFilter.MouseInput -= value; }
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
        public static event RawInputEventHandler Input
        {
            add { RawInputMessageFilter.Input += value; }
            remove { RawInputMessageFilter.Input -= value; }
        }

        #endregion
    }
}
