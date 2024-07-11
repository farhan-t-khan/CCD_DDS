using System;
using System.IO;
using System.Runtime.InteropServices;

namespace USBHID
{
    public class Kernel32
    {
        [DllImport("setupapi.dll")]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, Int32 Flags);

        [DllImport("setupapi.dll")]
        public static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInfo, ref Guid interfaceClassGuid, Int32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll")]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData, Int32 DeviceInterfaceDetailDataSize, ref Int32 RequiredSize, IntPtr DeviceInfoData);

        [DllImport("setupapi.dll")]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, Int32 DeviceInterfaceDetailDataSize, ref Int32 RequiredSize, IntPtr DeviceInfoData);

        [DllImport("setupapi.dll")]
        public static extern Boolean SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("Kernel32.dll")]
        public static extern IntPtr CreateFile(String lpFileName,
                                               [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess,
                                               [MarshalAs(UnmanagedType.U4)] FileShare  dwShareMode,
                                               Int32 lpSecurityAttributes,
                                               [MarshalAs(UnmanagedType.U4)] FileMode    dwCreationDisposition,
                                               [MarshalAs(UnmanagedType.U4)] FileOptions dwFlagsAndAttributes,
                                               IntPtr hTemplateFile);

        [DllImport("Kernel32.dll")]
        public static extern bool DeviceIoControl(IntPtr hDevice,
                                                  [MarshalAs(UnmanagedType.U4)] UInt32 dwIoControlCode,
                                                  IntPtr lpInBuffer,
                                                  [MarshalAs(UnmanagedType.U4)] UInt32 nInBufferSize,
                                                  IntPtr lpOutBuffer,
                                                  [MarshalAs(UnmanagedType.U4)] UInt32 nOutBufferSize,
                                                  [MarshalAs(UnmanagedType.U4)] out UInt32 lpBytesReturned,
                                                  IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern Boolean WriteFile(IntPtr hFile, [Out] Byte[] lpBuffer, UInt32 nNumberOfBytesToWrite, ref UInt32 lpNumberOfBytesWritten, IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern Boolean ReadFile(IntPtr hFile, [Out] Byte[] lpBuffer, UInt32 nNumberOfBytesToRead, ref UInt32 lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern Boolean CloseHandle(IntPtr hObject);

        //==========================================================================================================================================================================================================
        // DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA
        //==========================================================================================================================================================================================================
        public static IntPtr hardwareDeviceInfo;

        public const Int32  DIGCF_PRESENT          = 0x00000002;
        public const Int32  DIGCF_DEVICEINTERFACE  = 0x00000010;

        public const UInt32 GENERIC_READ           = 0x80000000;
        public const UInt32 GENERIC_WRITE          = 0x40000000;

        public const UInt32 FILE_SHARE_READ        = 0x00000001;
        public const UInt32 FILE_SHARE_WRITE       = 0x00000002;

        public const UInt32 OPEN_EXISTING          = 3;

        public enum FileMapProtection : UInt32
        {
            PageReadonly         = 0x02,
            PageReadWrite        = 0x04,
            PageWriteCopy        = 0x08,
            PageExecuteRead      = 0x20,
            PageExecuteReadWrite = 0x40,
            SectionCommit        = 0x8000000,
            SectionImage         = 0x1000000,
            SectionNoCache       = 0x10000000,
            SectionReserve       = 0x4000000,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LIST_ENTRY
        {
            public IntPtr Flink;
            public IntPtr Blink;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public  Int32   cbSize;
            public  Guid    interfaceClassGuid;
            public  Int32   flags;
            private UIntPtr reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public Int32 cbSize;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public String DevicePath;
        }
        //==========================================================================================================================================================================================================
        // END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA
        //==========================================================================================================================================================================================================
    }
}
