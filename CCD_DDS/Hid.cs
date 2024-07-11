using System;
using System.Runtime.InteropServices;

namespace USBHID
{
    public static class Hid
    {
        [DllImport("hid.dll")]
        public static extern void HidD_GetHidGuid(ref Guid Guid);

        [DllImport("hid.dll")]
        public static extern Boolean HidD_GetPreparsedData(IntPtr HidDeviceObject, ref IntPtr PreparsedData);

        [DllImport("hid.dll")]
        public static extern Boolean HidD_GetAttributes(IntPtr DeviceObject, ref HIDD_ATTRIBUTES Attributes);

        [DllImport("hid.dll")]
        public static extern UInt32 HidP_GetCaps(IntPtr PreparsedData, ref HIDP_CAPS Capabilities);

        [DllImport("hid.dll")]
        public static extern Boolean HidD_FreePreparsedData(ref IntPtr PreparsedData);

        [DllImport("hid.dll")]
        public static extern Boolean HidD_GetProductString(IntPtr HidDeviceObject, IntPtr Buffer, UInt32 BufferLength);

        [DllImport("hid.dll")]
        public static extern Boolean HidD_GetSerialNumberString(IntPtr HidDeviceObject, IntPtr Buffer, Int32 BufferLength);

        [DllImport("hid.dll")]
        public static extern Boolean HidD_GetManufacturerString(IntPtr HidDeviceObject, IntPtr Buffer, Int32 BufferLength);

        [DllImport("hid.dll")]
        public static extern Boolean HidD_GetHidReportDescriptor(IntPtr HidDeviceObject, IntPtr ReportDescriptor, Int32 ReportDescriptorLength);
    }

    //==========================================================================================================================================================================================================
    // DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA
    //==========================================================================================================================================================================================================
    public enum HIDP_REPORT_TYPE : UInt32
    {
        HidP_Input   = 0x00,
        HidP_Output  = 0x01,
        HidP_Feature = 0x02,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HIDP_CAPS
    {
        public UInt16   Usage;
        public UInt16   UsagePage;
        public UInt16   InputReportByteLength;
        public UInt16   OutputReportByteLength;
        public UInt16   FeatureReportByteLength;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public UInt16[] Reserved;

        public UInt16   NumberLinkCollectionNodes;

        public UInt16   NumberInputButtonCaps;
        public UInt16   NumberInputValueCaps;
        public UInt16   NumberInputDataIndices;

        public UInt16   NumberOutputButtonCaps;
        public UInt16   NumberOutputValueCaps;
        public UInt16   NumberOutputDataIndices;

        public UInt16   NumberFeatureButtonCaps;
        public UInt16   NumberFeatureValueCaps;
        public UInt16   NumberFeatureDataIndices;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct HIDD_ATTRIBUTES
    {
        public UInt32   Size;
        public UInt16   VendorID;
        public UInt16   ProductID;
        public UInt16   VersionNumber;
    }

    //[StructLayout(LayoutKind.Sequential)]
    //public struct HIDP_Range
    //{
    //    public UInt16   UsageMin,         UsageMax;
    //    public UInt16   StringMin,        StringMax;
    //    public UInt16   DesignatorMin,    DesignatorMax;
    //    public UInt16   DataIndexMin,     DataIndexMax;
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public struct HIDP_NotRange
    //{
    //    public UInt16   Usage,            Reserved1;
    //    public UInt16   StringIndex,      Reserved2;
    //    public UInt16   DesignatorIndex,  Reserved3;
    //    public UInt16   DataIndex,        Reserved4;
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public struct ButtonData
    //{
    //    public Int32    UsageMin;
    //    public Int32    UsageMax;
    //    public Int32    MaxUsageLength; 
    //    public Int16    Usages;
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public struct ValueData
    //{
    //    public UInt16   Usage;
    //    public UInt16   Reserved;

    //    public UInt64   Value;
    //    public Int64    ScaledValue;
    //}

    //[StructLayout(LayoutKind.Explicit)]
    //public struct HID_DATA
    //{
    //    [FieldOffset(0)]
    //    public Boolean      IsButtonData;
    //    [FieldOffset(1)]
    //    public Byte         Reserved;
    //    [FieldOffset(2)]
    //    public UInt16       UsagePage;
    //    [FieldOffset(4)]
    //    public Int32        Status;
    //    [FieldOffset(8)]
    //    public Int32        ReportID;
    //    [FieldOffset(16)]
    //    public Boolean      IsDataSet;

    //    [FieldOffset(17)]
    //    public ButtonData   ButtonData;
    //    [FieldOffset(17)]
    //    public ValueData    ValueData;
    //}

    public struct HID_DEVICE
    {
        public String             Manufacturer;
        public String             Product;
        public Int32              SerialNumber;
        public UInt16             VersionNumber;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public String             DevicePath;
        public IntPtr             Handle;

        //public Boolean            OpenedForRead;
        //public Boolean            OpenedForWrite;
        //public Boolean            OpenedOverlapped;
        //public Boolean            OpenedExclusive;

        public IntPtr             Ppd;
        public HIDP_CAPS          Caps;
        public HIDD_ATTRIBUTES    Attributes;

     //   public IntPtr[]           InputReportBuffer;
     //   public HID_DATA[]         InputData;
     //   public Int32              InputDataLength;

     //   public IntPtr[]           OutputReportBuffer;
     //   public HID_DATA[]         OutputData;
     //   public Int32              OutputDataLength;

     //   public IntPtr[]           FeatureReportBuffer;
	 //   public HID_DATA[]         FeatureData;
     //   public Int32              FeatureDataLength;
    }
    //==========================================================================================================================================================================================================
    // END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA
    //==========================================================================================================================================================================================================
}
