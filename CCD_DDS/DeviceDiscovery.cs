using System;
using System.IO;
using System.Runtime.InteropServices;

using static USBHID.Kernel32;

namespace USBHID
{
    public class DeviceDiscovery
    {
        static public Int32   FindDeviceNumber()
        {
            var hidGuid        = new Guid();
            var deviceInfoData = new SP_DEVICE_INTERFACE_DATA();

            Hid.HidD_GetHidGuid(ref hidGuid);

            //
            // Open a handle to the plug and play dev node.
            //
            SetupDiDestroyDeviceInfoList(Kernel32.hardwareDeviceInfo);
            hardwareDeviceInfo    = SetupDiGetClassDevs(ref hidGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
            deviceInfoData.cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DATA));

            var Index = 0;
            while (SetupDiEnumDeviceInterfaces(hardwareDeviceInfo, IntPtr.Zero, ref hidGuid, Index, ref deviceInfoData))
            {
                Index++;
            }

            return (Index);
        }
        static public void FindKnownHIDDevices(ref HID_DEVICE[] HID_Devices)
        {
            var hidGuid                 = new Guid();
            var deviceInfoData          = new SP_DEVICE_INTERFACE_DATA();
            var functionClassDeviceData = new SP_DEVICE_INTERFACE_DETAIL_DATA();

            Hid.HidD_GetHidGuid(ref hidGuid);

            //
            // Open a handle to the plug and play dev node.
            //
            SetupDiDestroyDeviceInfoList(hardwareDeviceInfo);
            hardwareDeviceInfo    = SetupDiGetClassDevs(ref hidGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
            deviceInfoData.cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DATA));

            for (var iHIDD = 0; iHIDD < HID_Devices.Length; iHIDD++)
            {
                SetupDiEnumDeviceInterfaces(hardwareDeviceInfo, IntPtr.Zero, ref hidGuid, iHIDD, ref deviceInfoData);

                //
                // Allocate a function class device data structure to receive the
                // goods about this particular device.
                //
                var RequiredLength = 0;
                SetupDiGetDeviceInterfaceDetail(hardwareDeviceInfo, ref deviceInfoData, IntPtr.Zero, 0, ref RequiredLength, IntPtr.Zero);

                if (IntPtr.Size == 8)
                {
                    functionClassDeviceData.cbSize = 8;
                }
                else if (IntPtr.Size == 4)
                {
                    functionClassDeviceData.cbSize = 5;
                }

                //
                // Retrieve the information from Plug and Play.
                //
                SetupDiGetDeviceInterfaceDetail(hardwareDeviceInfo, ref deviceInfoData, ref functionClassDeviceData, RequiredLength, ref RequiredLength, IntPtr.Zero);

                //
                // Open device with just generic query abilities to begin with
                //
                OpenHIDDevice(functionClassDeviceData.DevicePath, ref HID_Devices, iHIDD);
            }
        }
        static void OpenHIDDevice(String DevicePath, ref HID_DEVICE[] HID_Device, Int32 iHIDD)
        {
            //
            // RoutineDescription:
            // Given the HardwareDeviceInfo, representing a handle to the plug and
            // play information, and deviceInfoData, representing a specific hid device,
            // open that device and fill in all the relivant information in the given
            // HID_DEVICE structure.
            //
            HID_Device[iHIDD].DevicePath = DevicePath;

            //
            // The hid.dll api's do not pass the overlapped structure into deviceiocontrol
            // so to use them we must have a non overlapped device.  If the request is for
            // an overlapped device we will close the device below and get a handle to an
            // overlapped device
            //
            CloseHandle(HID_Device[iHIDD].Handle);
            HID_Device[iHIDD].Handle     = CreateFile(HID_Device[iHIDD].DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, 0, FileMode.Open, FileOptions.None, IntPtr.Zero);
            HID_Device[iHIDD].Caps       = new HIDP_CAPS();
            HID_Device[iHIDD].Attributes = new HIDD_ATTRIBUTES();

            //
            // If the device was not opened as overlapped, then fill in the rest of the
            // HID_Device structure.  However, if opened as overlapped, this handle cannot
            // be used in the calls to the HidD_ exported functions since each of these
            // functions does synchronous I/O.
            //
            Hid.HidD_FreePreparsedData(ref HID_Device[iHIDD].Ppd);
            HID_Device[iHIDD].Ppd = IntPtr.Zero;

            

            if ((int)HID_Device[iHIDD].Handle == -1)     // get out if invalid handle (needed this on Win7)
                return;

            Hid.HidD_GetPreparsedData(HID_Device[iHIDD].Handle, ref HID_Device[iHIDD].Ppd);
            Hid.HidD_GetAttributes   (HID_Device[iHIDD].Handle, ref HID_Device[iHIDD].Attributes);
            Hid.HidP_GetCaps         (HID_Device[iHIDD].Ppd   , ref HID_Device[iHIDD].Caps);

            var Buffer = Marshal.AllocHGlobal(126);
            {
                Hid.HidD_GetManufacturerString(HID_Device[iHIDD].Handle, Buffer, 126);
                HID_Device[iHIDD].Manufacturer = Marshal.PtrToStringAuto(Buffer);

                Hid.HidD_GetProductString(HID_Device[iHIDD].Handle, Buffer, 126);
                HID_Device[iHIDD].Product = Marshal.PtrToStringAuto(Buffer);

                Hid.HidD_GetSerialNumberString(HID_Device[iHIDD].Handle, Buffer, 126);
                HID_Device[iHIDD].SerialNumber = Marshal.PtrToStructure<Int32>(Buffer);
            }
            Marshal.FreeHGlobal(Buffer);
        }
    }
}
