// Copyright © 2020 Infinisis

using System;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Blaze.Interop.Win32;

namespace Blaze.Framework.RawInput
{
    static partial class RawInput
    {
        // == Get RawInput Code WParam ============================================================================= //

        /// <summary>
        ///   Gets the type of raw input received in a <c>WM_INPUT</c> message.
        /// </summary>
        /// <param name="wParam">The value of <see cref="Message.WParam"/>.</param>
        /// <returns>
        ///   <see cref="InputMode.Input"/> if the input is in the regular message flow;
        ///   or <see cref="InputMode.InputSink"/> if the input is sink only.
        /// </returns>
        /// <unmanaged>GET_RAWINPUT_CODE_WPARAM(wParam)</unmanaged>
        internal static InputMode GetRawInputCode(IntPtr wParam) => (InputMode) ((uint) wParam & 0xFF);


        // == Get RawInput Data ==================================================================================== //

        /// <summary>
        ///   Retrieves the size of the raw input data from the specified device.
        /// </summary>
        /// <param name="rawInputDataHandle">
        ///   The handle to the <see cref="RawInputData"/> structure. This comes from <see cref="Message.LParam"/> for
        ///   messages of type <c>WM_INPUT</c>.
        /// </param>
        /// <returns>
        ///   The number of bytes of raw input data available for the specified device.
        /// </returns>
        internal static unsafe uint GetRawInputDataSize(IntPtr rawInputDataHandle)
        {
            uint dataSize;
            GetRawInputData_((void*) rawInputDataHandle,
                             (uint) RawInputDataType.Input,
                             null,
                             &dataSize,
                             RawInputHeader.HeaderSize);
            return dataSize;
        }

        /// <summary>
        ///   Retrieves the raw input from the specified device.
        /// </summary>
        /// <param name="rawInputDataHandle">
        ///   The handle to the <see cref="RawInputData"/> structure. This comes from <see cref="Message.LParam"/> for
        ///   messages of type <c>WM_INPUT</c>.
        /// </param>
        /// <param name="rawInputData">
        ///   A correctly sized buffer (see <see cref="GetRawInputDataSize"/>) where to copy the data that comes from the
        ///   <see cref="RawInputData"/> structure.
        /// </param>
        /// <returns>
        ///   A reference to the <see cref="RawInputData"/> structure.
        /// </returns>
        /// <remarks>
        ///   This function gets the raw input one <see cref="RawInputData"/> structure at a time. In contrast,
        ///   <see cref="GetRawInputBuffer"/> gets an array of <see cref="RawInputData"/> structures.
        /// </remarks>
        internal static unsafe ref RawInputData GetRawInputData(IntPtr rawInputDataHandle, Span<byte> rawInputData)
        {
            var dataSize = (uint) rawInputData.Length;

            fixed (void* ptrRawInputData = &rawInputData[0])
            {
                GetRawInputData_((void*) rawInputDataHandle,
                                 (uint) RawInputDataType.Input,
                                 ptrRawInputData,
                                 &dataSize,
                                 RawInputHeader.HeaderSize);

                RawInputData* rawInputPtr = (RawInputData*) ptrRawInputData;
                return ref *rawInputPtr;
            }
        }

        /// <unmanaged>unsigned int GetRawInputData([In] HRAWINPUT hRawInput,[In] unsigned int uiCommand,[Out, Buffer, Optional] void* pData,[InOut] unsigned int* pcbSize,[In] unsigned int cbSizeHeader)</unmanaged>
        [DllImport("user32.dll", EntryPoint = "GetRawInputData", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int GetRawInputData_(void* hRawInput, uint uiCommand, void* pData, uint* pcbSize, uint cbSizeHeader);


        // == Get RawInput Data (Buffered) ========================================================================= //

        /// <summary>
        ///   Performs a buffered read of the raw input data.
        /// </summary>
        /// <param name="dataBuffer">
        ///   A reference to a buffer of <see cref="RawInputData"/> structures that contain the raw input data. If <c>null</c>, the
        ///   minimum required buffer, in bytes, is returned in <paramref name="dataSize"/>.
        /// </param>
        /// <param name="dataSize">The size of a <see cref="RawInputData"/> structure, in bytes.</param>
        /// <param name="dataHeaderSize">The size of the <see cref="RawInputData.RawInputHeader" /> structure, in bytes.</param>
        /// <returns>
        ///   If <paramref name="dataBuffer"/> is <c>null</c> and the function is successful, the return value is zero.
        ///   If it is not <c>null</c> and the function is successful, the return value is the number of <see cref="RawInputData"/>
        ///   structures written.
        ///   If an error occurs, the return value is -1.
        /// </returns>
        /// <remarks>
        ///   Using this function, the raw input data is buffered in the array of <see cref="RawInputData"/> structures.
        ///   The NEXTRAWINPUTBLOCK macro allows an application to traverse an array of <see cref="RawInputData"/> structures.
        /// </remarks>
        /// <unmanaged>unsigned int GetRawInputBuffer([Out, Buffer, Optional] RAWINPUT* pData,[InOut] unsigned int* pcbSize,[In] unsigned int cbSizeHeader)</unmanaged>
        internal static unsafe int GetRawInputBuffer(RawInputData[] dataBuffer, ref uint dataSize, uint dataHeaderSize)
        {
            int result;
            var nativeArray = dataBuffer is null ? null : new RawInputData[dataBuffer.Length];

            fixed (uint* ptrDataSize = &dataSize)
            fixed (RawInputData* ptrNativeArray = nativeArray)
                result = GetRawInputBuffer_(ptrNativeArray, ptrDataSize, dataHeaderSize);

            if (dataBuffer != null)
            {
                for (int index = 0; index < dataBuffer.Length; ++index)
                    dataBuffer[index] = nativeArray[index];
            }
            return result;
        }

        [DllImport("user32.dll", EntryPoint = "GetRawInputBuffer", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int GetRawInputBuffer_(void* pData, uint* pcbSize, uint cbSizeHeader);


        // == Get RawInput Device Info ============================================================================= //

        /// <summary>
        ///   Retrieves information about the raw input device.
        /// </summary>
        /// <param name="handle">
        ///   A handle to the raw input device. This comes from <see cref="RawInputData.RawInputHeader.Device" /> or from
        ///   <see cref="GetRawInputDeviceList"/>.
        /// </param>
        /// <param name="infoType">Specifies what data will be returned in <paramref name="infoData"/>.</param>
        /// <param name="infoData">
        ///   A reference to a buffer that contains the information specified by <paramref name="infoType"/>.
        ///   If it is <see cref="RawInputDeviceInfoType.DeviceInfo"/>, set <see cref="RawDeviceInformation.Size" />
        ///   to <c>sizeof(<see cref="RawDeviceInformation"/>)</c> before calling this function.
        /// </param>
        /// <param name="infoDataSize">The size of the data in <paramref name="infoData"/>, in bytes.</param>
        /// <returns>
        ///   If successful, this function returns a non-negative number indicating the number of bytes copied to
        ///   <paramref name="infoData"/>. If the buffer is not large enough for the data, the function returns -1.
        ///   <para/>
        ///   If <paramref name="infoData"/> is <c>null</c>, the function returns a value of zero. In both of these
        ///   cases, <paramref name="infoDataSize"/> is set to the minimum size required for the data buffer.
        /// </returns>
        /// <unmanaged>unsigned int GetRawInputDeviceInfoW([In, Optional] void* hDevice,[In] unsigned int uiCommand,[Out, Buffer, Optional] void* pData,[InOut] unsigned int* pcbSize)</unmanaged>
        internal static unsafe int GetRawInputDeviceInfo(IntPtr handle, RawInputDeviceInfoType infoType, IntPtr infoData, ref uint infoDataSize)
        {
            fixed (uint* ptrDataSize = &infoDataSize)
                return GetRawInputDeviceInfoW_((void*) handle, (uint) infoType, (void*) infoData, ptrDataSize);
        }

        [DllImport("user32.dll", EntryPoint = "GetRawInputDeviceInfoW", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int GetRawInputDeviceInfoW_(void* hDevice, uint uiCommand, void* pData, uint* pcbSize);


        // == Register RawInput Devices ============================================================================ //

        /// <summary>
        ///   Registers the devices that supply the raw input data.
        /// </summary>
        /// <param name="rawInputDevices">
        ///   An array of <see cref="RawInputDevice" /> structures that represent the devices that supply the raw input.
        /// </param>
        /// <returns>
        ///   <see cref="Result.Ok" /> if the function succeeds; otherwise, <see cref="Result.False" />.
        /// </returns>
        /// <remarks>
        ///   To receive <c>WM_INPUT</c> messages, an application must first register the raw input devices using
        ///   <see cref="RegisterRawInputDevices"/>. By default, an application does not receive raw input.
        ///   <para/>
        ///   To receive <c>WM_INPUT_DEVICE_CHANGE</c> messages, an application must specify <see cref="DeviceFlags.DeviceNotify"/>
        ///   for each device class that is specified by the <see cref="UsagePage"/> and <see cref="UsageId"/>
        ///   of the  <see cref="RawInputDevice" /> structure. By default, an application does not receive
        ///   <c>WM_INPUT_DEVICE_CHANGE</c> notifications for raw input device arrival and removal.
        ///   <para/>
        ///   If a <see cref="RawInputDevice" /> structure has the <see cref="DeviceFlags.Remove"/> flag set and
        ///   <see cref="RawInputDevice.Target"/> is not set to <c>null</c>, then parameter validation will fail.
        /// </remarks>
        /// <unmanaged>BOOL RegisterRawInputDevices([In, Buffer] const RAWINPUTDEVICE* pRawInputDevices,[In] unsigned int uiNumDevices,[In] unsigned int cbSize)</unmanaged>
        internal static unsafe Bool RegisterRawInputDevices(ReadOnlySpan<RawInputDevice> rawInputDevices)
        {
            var structSize = (uint) Unsafe.SizeOf<RawInputDevice>();
            var deviceCount = (uint) rawInputDevices.Length;

            fixed (RawInputDevice* ptrRawInputDevices = rawInputDevices)
                return RegisterRawInputDevices_(ptrRawInputDevices, deviceCount, structSize);
        }

        [DllImport("user32.dll", EntryPoint = "RegisterRawInputDevices", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe Bool RegisterRawInputDevices_(void* pRawInputDevices, uint uiNumDevices, uint cbSize);


        // == Get RawInput Registered Devices ====================================================================== //

        /// <summary>
        ///   Retrieves the information about the raw input devices for the current application.
        /// </summary>
        /// <param name="rawInputDevices">An array of <see cref="RawInputDevice" /> structures for the application.</param>
        /// <param name="numDevices">The number of <see cref="RawInputDevice" /> structures in <paramref name="rawInputDevices"/>.</param>
        /// <param name="rawInputDeviceSize">The size of a <see cref="RawInputDevice" /> structure, in bytes.</param>
        /// <returns>
        ///   If successful, the function returns a non-negative number that is the number of <see cref="RawInputDevice" />
        ///   structures written to the buffer.
        ///   <para/>
        ///   If the  buffer is too small or <c>null</c>, the function returns -1, and sets <paramref name="numDevices"/>
        ///   to the required number of devices.
        ///   <para/>
        ///   If the function fails for any other reason, it returns -1.</returns>
        /// <remarks>
        ///   To receive raw input from a device, an application must register it by using <see cref="RegisterRawInputDevices"/>.
        /// </remarks>
        /// <unmanaged>unsigned int GetRegisteredRawInputDevices([Out, Buffer, Optional] RAWINPUTDEVICE* pRawInputDevices,[InOut] unsigned int* puiNumDevices,[In] unsigned int cbSize)</unmanaged>
        internal static unsafe int GetRegisteredRawInputDevices(RawInputDevice[] rawInputDevices, ref uint numDevices, uint rawInputDeviceSize)
        {
            fixed (uint* ptrNumDevices = &numDevices)
            fixed (RawInputDevice* ptrRawInputDevices = rawInputDevices)
                return GetRegisteredRawInputDevices_(ptrRawInputDevices, ptrNumDevices, rawInputDeviceSize);
        }

        [DllImport("user32.dll", EntryPoint = "GetRegisteredRawInputDevices", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int GetRegisteredRawInputDevices_(void* pRawInputDevices, uint* puiNumDevices, uint cbSize);


        // == Get RawInput Devices ================================================================================= //

        /// <summary>
        ///   Enumerates the raw input devices attached to the system.
        /// </summary>
        /// <param name="rawInputDevices">
        ///   An array of <see cref="RawInputDeviceList" /> structures for the devices attached to the system. If <c>null</c>,
        ///   the number of devices are returned in <paramref name="numDevices"/>.
        /// </param>
        /// <param name="numDevices">
        ///   If <paramref name="rawInputDevices"/> is <c>null</c>, the function populates this variable with the number
        ///   of devices attached to the system; otherwise, this variable specifies the number of <see cref="RawInputDeviceList" />
        ///   structures that can be contained in the buffer. If this value is less than the number of devices attached to the
        ///   system, the function returns the actual number of devices in this variable and fails.
        /// </param>
        /// <param name="rawInputDeviceListSize">The size of a <see cref="RawInputDeviceList" /> structure, in bytes.</param>
        /// <returns>
        ///   If the function is successful, the return value is the number of devices stored in the buffer pointed to by
        ///   <paramref name="rawInputDevices"/>.
        ///   On any other error, the function returns -1.
        /// </returns>
        /// <remarks>
        ///   The devices returned from this function are the mouse, the keyboard, and other Human Input Device (HID) devices.
        ///   To get more detailed information about the attached devices, call <see cref="GetRawInputDeviceInfo"/> using the
        ///   <see cref="RawInputDeviceList.Device"/>.
        /// </remarks>
        /// <unmanaged>unsigned int GetRawInputDeviceList([Out, Buffer, Optional] RAWINPUTDEVICELIST* pRawInputDeviceList,[InOut] unsigned int* puiNumDevices,[In] unsigned int cbSize)</unmanaged>
        internal static unsafe int GetRawInputDeviceList(RawInputDeviceList[] rawInputDevices, ref uint numDevices, uint rawInputDeviceListSize)
        {
            fixed (uint* ptrNumDevices = &numDevices)
            fixed (RawInputDeviceList* ptrRawInputDeviceList = rawInputDevices)
                return GetRawInputDeviceList_(ptrRawInputDeviceList, ptrNumDevices, rawInputDeviceListSize);
        }

        [DllImport("user32.dll", EntryPoint = "GetRawInputDeviceList", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int GetRawInputDeviceList_(void* pRawInputDeviceList, uint* puiNumDevices, uint cbSize);
    }
}
