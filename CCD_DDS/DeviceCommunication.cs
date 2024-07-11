using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace USBHID
{
    public class DeviceCommunication
    {
        //public struct HIDReadInfo
        //{
        //    public static List<Byte>    ReportData;

        //    public static Boolean       Active;

        //    public static HID_DEVICE[]  Device;
        //    public static Int32         iDevice;

        //    public static UInt16        VendorID;
        //    public static UInt16        ProductID;
        //}
        //public struct HIDWriteInfo
        //{
        //    public static Byte[]        ReportData = new Byte[8];
        //    public static Byte          ReportID;

        //    public static Boolean       Done;
        //    public static Boolean       Active;

        //    public static HID_DEVICE[]  Device;
        //    public static Int32         iDevice;

        //    public static UInt16        VendorID;
        //    public static UInt16        ProductID;
        //    public static UInt16        UsagePage;
        //    public static UInt16        Usage;
        //}

        //static async public void BeginAsyncRead(object state)
        //{
        //    //
        //    // Read what the USB device has sent to the PC and store the result inside HID_Report[]
        //    //
        //    if (HIDReadInfo.Active == true)
        //    {
        //        var Device       = HIDReadInfo.Device[HIDReadInfo.iDevice];
        //        var ReportBuffer = new Byte[Device.Caps.InputReportByteLength];

        //        if (ReportBuffer.Length > 0)
        //        {
        //            await Task.Run(() =>
        //            {
        //                var NumberOfBytesRead = 0U;
        //                Kernel32.ReadFile(Device.Handle, ReportBuffer, Device.Caps.InputReportByteLength, ref NumberOfBytesRead, IntPtr.Zero);
        //            });

        //            HIDReadInfo.ReportData = new List<Byte>(ReportBuffer);
        //        }
        //    }
        //}
        //static public void BeginSyncSend(object state)
        //{
        //    //
        //    // Sent to the USB device what is stored inside WriteData[]
        //    //
        //    if (HIDWriteInfo.Done == false && HIDWriteInfo.Active == true)
        //    {
        //        var Device = HIDWriteInfo.Device[HIDWriteInfo.iDevice];
        //        var ReportBuffer = new Byte[Device.Caps.OutputReportByteLength];

        //        if (ReportBuffer.Length > 0)
        //        {
        //            //
        //            // Add ReportID to the first byte of HID_ReportContent
        //            //
        //            ReportBuffer[0] = HIDWriteInfo.ReportID;

        //            //
        //            // Copy ReportData into HID_ReportContent starting from index 1
        //            //
        //            Array.Copy(HIDWriteInfo.ReportData, 0, ReportBuffer, 1, ReportBuffer.Length - 1);

        //            var varA = 0U;
        //            Kernel32.WriteFile(Device.Handle, ReportBuffer, Device.Caps.OutputReportByteLength, ref varA, IntPtr.Zero);
        //        }

        //        HIDWriteInfo.Done = true;
        //    }
        //}
    }
}