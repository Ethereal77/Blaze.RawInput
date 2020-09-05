// Copyright © 2020 Infinisis

using System;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Blaze.Interop.Win32;

using static Blaze.Framework.RawInput.ThrowHelper;

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
        /// <returns>The number of <see cref="RawInputData"/> structures written to the buffer.</returns>
        /// <remarks>
        ///   Using this function, the raw input data is buffered in the buffer of <see cref="RawInputData"/> structures.
        ///   The <see cref="NextRawInputBlock(in RawInputData)"/> function allows an application to traverse the array
        ///   of <see cref="RawInputData"/> structures.
        /// </remarks>
        internal static unsafe uint GetRawInputBuffer(Span<byte> rawInputDataBuffer)
        {
            var rawInputSize = (uint) rawInputDataBuffer.Length;

            int result;
            fixed (byte* ptrDataBuffer = &rawInputDataBuffer[0])
                result = GetRawInputBuffer_(ptrDataBuffer, &rawInputSize, RawInputHeader.HeaderSize);

            CheckResult(result);

            return (uint) result;
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
        /// <param name="ptrRawInput">A pointer to a structure in an array of <see cref="RawInputData"/> structures.</param>
        /// <unmanaged>NEXTRAWINPUTBLOCK(ptr)</unmanaged>
        private static unsafe RawInputData* NextRawInputBlock(RawInputData* ptrRawInput)
        {
            // ((PRAWINPUT)RAWINPUT_ALIGN((ULONG_PTR)((PBYTE)(ptr) + (ptr)->header.dwSize)))
            return (RawInputData*) RawInputAlign((byte*) ptrRawInput + ptrRawInput->Header.Size);
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
        ///   Gets the name of a raw input device.
        /// </summary>
        /// <param name="deviceHandle">
        ///   A handle to the raw input device. This comes from <see cref="RawInputHeader.Device" /> or from
        ///   <see cref="GetRawInputDeviceList"/>.
        /// </param>
        /// <returns>The name of the device.</returns>
        internal static unsafe string GetRawInputDeviceName(IntPtr deviceHandle)
        {
            uint nameLength;
            Result result = GetRawInputDeviceInfoW_((void*) deviceHandle,
                                                    (uint) RawInputDeviceInfoType.DeviceName,
                                                    null,
                                                    &nameLength);
            CheckResult(result);

            Span<char> nameData = stackalloc char[(int) nameLength];
            fixed(char* ptrNameData = &nameData[0])
                result = GetRawInputDeviceInfoW_((void*) deviceHandle,
                                                 (uint) RawInputDeviceInfoType.DeviceName,
                                                 ptrNameData,
                                                 &nameLength);

            CheckResult(result);

            int index0 = nameData.IndexOf('\0');
            return index0 >= 0
                ? nameData[0..index0].ToString()
                : nameData.ToString();
        }

        /// <summary>
        ///   Gets the size of the buffer required to get a <see cref="RawDeviceInformation"/> structure
        ///   when calling <see cref="GetRawInputDeviceInfo"/>.
        /// </summary>
        /// <param name="deviceHandle">
        ///   A handle to the raw input device. This comes from <see cref="RawInputHeader.Device" /> or from
        ///   <see cref="GetRawInputDeviceList"/>.
        /// </param>
        /// <returns>The required size of the buffer to pass to <see cref="GetRawInputDeviceInfo"/>.</returns>
        internal static unsafe uint GetRawInputDeviceInfoSize(IntPtr deviceHandle)
        {
            uint deviceInfoSize;
            Result result = GetRawInputDeviceInfoW_((void*) deviceHandle,
                                                    (uint) RawInputDeviceInfoType.DeviceInfo,
                                                    null,
                                                    &deviceInfoSize);

            CheckResult(result);

            return deviceInfoSize;
        }

        /// <summary>
        ///   Retrieves information about a raw input device.
        /// </summary>
        /// <param name="deviceHandle">
        ///   A handle to the raw input device. This comes from <see cref="RawInputHeader.Device" /> or from
        ///   <see cref="GetRawInputDeviceList"/>.
        /// </param>
        /// <param name="deviceInfoData">
        ///   A correctly sized buffer (see <see cref="GetRawInputDeviceInfoSize"/>) where to copy the data that comes from the
        ///   <see cref="RawDeviceInformation"/> structure.
        /// </param>
        /// <returns>A reference to the <see cref="RawDeviceInformation"/> structure.</returns>
        internal static unsafe ref RawDeviceInformation GetRawInputDeviceInfo(IntPtr deviceHandle, Span<byte> deviceInfoData)
        {
            uint deviceInfoSize = (uint) deviceInfoData.Length;

            fixed (byte* ptrDeviceInfo = &deviceInfoData[0])
            {
                Result result = GetRawInputDeviceInfoW_((void*) deviceHandle,
                                                        (uint) RawInputDeviceInfoType.DeviceInfo,
                                                        ptrDeviceInfo,
                                                        &deviceInfoSize);

                CheckResult(result);

                var deviceInfoPtr = (RawDeviceInformation*) ptrDeviceInfo;
                return ref *deviceInfoPtr;
            }
        }

        /// <unmanaged>unsigned int GetRawInputDeviceInfoW([In, Optional] void* hDevice,[In] unsigned int uiCommand,[Out, Buffer, Optional] void* pData,[InOut] unsigned int* pcbSize)</unmanaged>
        [DllImport("user32.dll", EntryPoint = "GetRawInputDeviceInfoW", SetLastError = true, PreserveSig = true, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int GetRawInputDeviceInfoW_(void* hDevice, uint uiCommand, void* pData, uint* pcbSize);


        // == Register RawInput Devices ============================================================================ //

        /// <summary>
        ///   Registers a device that supplies the raw input data.
        /// </summary>
        /// <param name="rawInputDevice">A <see cref="RawInputDevice" /> structure that represent the device to register.</param>
        /// <returns><c>true</c> if the function succeeds; otherwise, <c>false</c>.</returns>
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
        ///   If a <see cref="RawInputDevice" /> structure has the <see cref="DeviceFlags.Remove"/> flag set and
        ///   <see cref="RawInputDevice.Target"/> is not set to <c>null</c>, then parameter validation will fail.
        /// </remarks>
        internal static unsafe bool RegisterRawInputDevice(RawInputDevice rawInputDevice)
        {
            var structSize = (uint) Unsafe.SizeOf<RawInputDevice>();

            return RegisterRawInputDevices_(&rawInputDevice,
                                            uiNumDevices: 1,
                                            cbSize: structSize);
        }

        /// <unmanaged>BOOL RegisterRawInputDevices([In, Buffer] const RAWINPUTDEVICE* pRawInputDevices,[In] unsigned int uiNumDevices,[In] unsigned int cbSize)</unmanaged>
        [DllImport("user32.dll", EntryPoint = "RegisterRawInputDevices", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe Bool RegisterRawInputDevices_(void* pRawInputDevices, uint uiNumDevices, uint cbSize);


        // == Get RawInput Registered Devices ====================================================================== //

        /// <summary>
        ///   Gets the number of raw input devices registered for the current application.
        /// </summary>
        /// <returns>
        ///   The number of currently registered devices.
        /// <remarks>
        ///   To receive raw input from a device, an application must register it by using <see cref="RegisterRawInputDevice"/>.
        /// </remarks>
        internal static unsafe uint GetRegisteredRawInputDeviceCount()
        {
            uint numDevices = 0;

            Result result = GetRegisteredRawInputDevices_(null, &numDevices, (uint) Unsafe.SizeOf<RawInputDevice>());

            CheckResult(result);

            return numDevices;
        }

        /// <summary>
        ///   Retrieves the information about the raw input devices registered for the current application.
        /// </summary>
        /// <param name="rawInputDevices">
        ///   A correctly sized buffer (see <see cref="GetRegisteredRawInputDeviceCount"/>) where to copy the data that comes from
        ///   the <see cref="RawInputDevice"/> structures for the devices registered for the current application.
        /// </param>
        /// <remarks>
        ///   To receive raw input from a device, an application must register it by using <see cref="RegisterRawInputDevice"/>.
        /// </remarks>
        internal static unsafe void GetRegisteredRawInputDevices(Span<RawInputDevice> rawInputDevices)
        {
            var numDevices = (uint) rawInputDevices.Length;

            fixed (RawInputDevice* ptrRawInputDevices = &rawInputDevices[0])
            {
                Result result = GetRegisteredRawInputDevices_(ptrRawInputDevices,
                                                              &numDevices,
                                                              (uint) Unsafe.SizeOf<RawInputDevice>());

                CheckResult(result);
            }
        }

        /// <unmanaged>unsigned int GetRegisteredRawInputDevices([Out, Buffer, Optional] RAWINPUTDEVICE* pRawInputDevices,[InOut] unsigned int* puiNumDevices,[In] unsigned int cbSize)</unmanaged>
        [DllImport("user32.dll", EntryPoint = "GetRegisteredRawInputDevices", SetLastError = true, PreserveSig = true, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int GetRegisteredRawInputDevices_(void* pRawInputDevices, uint* puiNumDevices, uint cbSize);


        // == Get RawInput Devices ================================================================================= //

        /// <summary>
        ///   Gets the number of raw input devices attached to the system.
        /// </summary>
        /// <returns>The number of devices attached to the system.</returns>
        /// <remarks>
        ///   The devices counted by this function are the mouse, the keyboard, and other Human Input Device (HID) devices.
        ///   To get more detailed information about the attached devices, call <see cref="GetRawInputDeviceInfo"/> using the
        ///   <see cref="RawInputDeviceList.Device"/>.
        /// </remarks>
        internal static unsafe uint GetRawInputDeviceCount()
        {
            uint numDevices = 0;

            Result result = GetRawInputDeviceList_(null, &numDevices, (uint) Unsafe.SizeOf<RawInputDeviceList>());

            CheckResult(result);

            return numDevices;
        }

        /// <summary>
        ///   Enumerates the raw input devices attached to the system.
        /// </summary>
        /// <param name="rawInputDevices">
        ///   A correctly sized buffer (see <see cref="GetRawInputDeviceCount"/>) where to copy the data that comes from the
        ///   <see cref="RawInputDeviceList"/> structures for the devices attached to the system.
        /// </param>
        /// <remarks>
        ///   The devices returned from this function are the mouse, the keyboard, and other Human Input Device (HID) devices.
        ///   To get more detailed information about the attached devices, call <see cref="GetRawInputDeviceInfo"/> using the
        ///   <see cref="RawInputDeviceList.Device"/>.
        /// </remarks>
        internal static unsafe void GetRawInputDeviceList(Span<RawInputDeviceList> rawInputDevices)
        {
            var numDevices = (uint) rawInputDevices.Length;

            fixed (RawInputDeviceList* ptrRawInputDevices = &rawInputDevices[0])
            {
                Result result = GetRawInputDeviceList_(ptrRawInputDevices,
                                                       &numDevices,
                                                       (uint) Unsafe.SizeOf<RawInputDeviceList>());

                CheckResult(result);
            }
        }

        /// <unmanaged>unsigned int GetRawInputDeviceList([Out, Buffer, Optional] RAWINPUTDEVICELIST* pRawInputDeviceList,[InOut] unsigned int* puiNumDevices,[In] unsigned int cbSize)</unmanaged>
        [DllImport("user32.dll", EntryPoint = "GetRawInputDeviceList", SetLastError = true, PreserveSig = true, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int GetRawInputDeviceList_(void* pRawInputDeviceList, uint* puiNumDevices, uint cbSize);
    }
}
