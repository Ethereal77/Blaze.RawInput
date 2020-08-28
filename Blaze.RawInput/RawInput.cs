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

        private static DeviceInfoCollection g_Devices;

        /// <summary>
        ///   Gets a collection of the available RawInput devices attached to the system.
        /// </summary>
        public static DeviceInfoCollection Devices => g_Devices ??= new DeviceInfoCollection();

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

        /// <summary>
        ///   Occurs when a raw input device has been added to or removed from the system.
        /// </summary>
        /// <remarks>
        ///   This event provides the handle of the device that was attached or removed from the system.
        ///   If the change is an arrival of a new device attached to the system, you can get information
        ///   about the device from <see cref="Devices"/>.
        /// </remarks>
        public static event DeviceChangedEventHandler DeviceChanged
        {
            add { RawInputMessageFilter.DeviceChanged += value; }
            remove { RawInputMessageFilter.DeviceChanged -= value; }
        }

        #endregion
    }
}
