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
        ///   Queries the minimum size of the buffer of <see cref="RawInputData"/> structures, including any extra
        ///   data needed for HID devices.
        /// </summary>
        /// <returns>
        ///   Minimum required buffer size, in bytes.
        /// </returns>
        /// <remarks>
        ///   Using this function to size the buffer for reading raw input data with <see cref="GetRawInputBuffer"/>.
        ///   A good buffer size might be 8x or 16x the size returned by this function.
        /// </remarks>
        internal static unsafe uint GetRawInputBufferSize()
        {
            uint dataSize;
            GetRawInputBuffer_(null, &dataSize, RawInputHeader.HeaderSize);
            return dataSize;
        }

        /// <summary>
        ///   Performs a buffered read of the raw input data.
        /// </summary>
        /// <param name="rawInputDataBuffer">A buffer to contain the raw input data.</param>
        /// <returns>
        ///   A <see cref="Result"/> code:
        ///   <list type="bullet">
        ///     <item>
        ///       If <paramref name="rawInputDataBuffer"/> is empty and the function is successful, the return value is <see cref="Result.Ok"/>.
        ///     </item>
        ///     <item>
        ///       If it is not empty and the function is successful, the return value is the number of <see cref="RawInputData"/> structures
        ///       written to the buffer.
        ///     </item>
        ///     <item>If an error occurs, the return value is the Win32 error code.</item>
        ///   </list>
        /// </returns>
        /// <remarks>
        ///   Using this function, the raw input data is buffered in the buffer of <see cref="RawInputData"/> structures.
        ///   The <see cref="NextRawInputBlock(in RawInputData)"/> function allows an application to traverse the array
        ///   of <see cref="RawInputData"/> structures.
        /// </remarks>
        internal static unsafe Result GetRawInputBuffer(Span<byte> rawInputDataBuffer)
        {
            var rawInputSize = (uint) rawInputDataBuffer.Length;

            int result;
            fixed (byte* ptrDataBuffer = &rawInputDataBuffer[0])
                result = GetRawInputBuffer_(ptrDataBuffer, &rawInputSize, RawInputHeader.HeaderSize);

            return result >= 0 ? result : Result.FromWin32Error(Marshal.GetLastWin32Error());
        }

        /// <summary>
        ///   Returns the provided pointer aligned to the word size of the current process architecture
        ///   (x86 or AMD64).
        /// </summary>
        /// <param name="ptr">The pointer to align.</param>
        /// <returns>The aligned pointer.</returns>
        /// <unmanaged>RAWINPUT_ALIGN(x)</unmanaged>
        private static unsafe byte* RawInputAlign(byte* ptr)
        {
            if(Environment.Is64BitProcess)
            {
                // #define RAWINPUT_ALIGN(x)   (((x) + sizeof(QWORD) - 1) & ~(sizeof(QWORD) - 1))
                return (byte*) (((long) (ptr + sizeof(long) - 1)) & ~(sizeof(long) - 1));
            }
            else
            {
                // #define RAWINPUT_ALIGN(x)   (((x) + sizeof(DWORD) - 1) & ~(sizeof(DWORD) - 1))
                return (byte*) (((int) (ptr + sizeof(int) - 1)) & ~(sizeof(int) - 1));
            }
        }

        /// <summary>
        ///   Retrieves the location of the next structure in an array of <see cref="RawInputData"/> structures.
        /// </summary>
        /// <param name="ptr">A pointer to a structure in an array of <see cref="RawInputData"/> structures.</param>
        /// <unmanaged>NEXTRAWINPUTBLOCK(ptr)</unmanaged>
        internal static unsafe RawInputData* NextRawInputBlock(RawInputData* ptr)
        {
            // ((PRAWINPUT)RAWINPUT_ALIGN((ULONG_PTR)((PBYTE)(ptr) + (ptr)->header.dwSize)))
            return (RawInputData*) RawInputAlign((byte*)ptr + ptr->Header.Size);
        }

        /// <summary>
        ///   Retrieves the location of the next structure in an array of <see cref="RawInputData"/> structures.
        /// </summary>
        /// <param name="rawInput">A reference to a structure in an array of <see cref="RawInputData"/> structures.</param>
        /// <unmanaged>NEXTRAWINPUTBLOCK(ptr)</unmanaged>
        internal static unsafe ref RawInputData NextRawInputBlock(in RawInputData rawInput)
        {
            var dataPtr = (RawInputData*) Unsafe.AsPointer(ref Unsafe.AsRef(in rawInput));
            dataPtr = NextRawInputBlock(dataPtr);
            return ref Unsafe.AsRef<RawInputData>(dataPtr);
        }

        /// <unmanaged>unsigned int GetRawInputBuffer([Out, Buffer, Optional] RAWINPUT* pData,[InOut] unsigned int* pcbSize,[In] unsigned int cbSizeHeader)</unmanaged>
        [DllImport("user32.dll", EntryPoint = "GetRawInputBuffer", SetLastError = true, PreserveSig = true, CallingConvention = CallingConvention.StdCall)]
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
