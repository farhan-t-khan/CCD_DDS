using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace USBHID
{
    public partial class Core
    {
/*        [StructLayout(LayoutKind.Sequential)]
        public struct Flash_Root
        {
            public byte type;
            public UInt32 directory_start_page;
            public UInt32 pages_in_directory;
            public UInt16 size_of_pages;
            public UInt32 pages_in_memory;
            public byte[] unit_serial_number;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)] public byte[] unit_model_number;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public byte[] CPU_ID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public byte[] data_base_revision;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] instrument_name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public byte[] revision_of_instrument_software;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public byte[] ms_sensor_part_number;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public byte[] co_sensor_part_number;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public byte[] o2_sensor_part_number;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public byte[] hs_sensor_part_number;
            public byte status;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 113)] public byte[] pad;
        }*/

        private ICommunication communication;

        public string? Model;
        public string? Serial;

        public string? CalData;

        public Core()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Entry);

            USBCommunication usbCommunication = new USBCommunication();
            //BluetoothCommunication bluetoothCommunication = new BluetoothCommunication("COM1");

            if (usbCommunication.DetectRovEx())
            {
                communication = usbCommunication;
                usbCommunication.Setup();
                Model = usbCommunication.Model;
                Serial = usbCommunication.Serial;

                //Log Cal data for debug
                CalData = usbCommunication.readCalibrationResults();
                Console.WriteLine(CalData);
            }
/*            else if (bluetoothCommunication.DetectRovEx())
            {
                communication = bluetoothCommunication;
                bluetoothCommunication.Setup();
            }*/
            else
            {
                
            }
        }
        private void Entry(object sender, RoutedEventArgs e)
        {
            // Perform actions after the window has loaded
        }

        public void SendPacket(byte[] pkt)
        {
            communication.SendPacket(pkt);
        }

        public uint ReceivePacket(ref byte[] buf, uint size)
        {
            return communication.ReceivePacket(ref buf, size);
        }

        public void ReadFlashBlock(uint blockNumber, ref byte[] buf)
        {
            communication.ReadFlashBlock(blockNumber, ref buf);
        }

        void SetByte0(object sender, TextChangedEventArgs e)
        {
            //    var PacketValue = sender as TextBox;

            //    Byte.TryParse(PacketValue.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out HIDWriteInfo.ReportData[0]);
        }
        void SetByte1(object sender, TextChangedEventArgs e)
        {
            //    var PacketValue = sender as TextBox;

            //    Byte.TryParse(PacketValue.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out HIDWriteInfo.ReportData[1]);
        }
        void SetByte2(object sender, TextChangedEventArgs e)
        {
            //    var PacketValue = sender as TextBox;

            //    Byte.TryParse(PacketValue.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out HIDWriteInfo.ReportData[2]);
        }
        void SetByte3(object sender, TextChangedEventArgs e)
        {
            //    var PacketValue = sender as TextBox;

            //    Byte.TryParse(PacketValue.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out HIDWriteInfo.ReportData[3]);
        }
        void SetByte4(object sender, TextChangedEventArgs e)
        {
            //    var PacketValue = sender as TextBox;

            //    Byte.TryParse(PacketValue.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out HIDWriteInfo.ReportData[4]);
        }
        void SetByte5(object sender, TextChangedEventArgs e)
        {
            //    var PacketValue = sender as TextBox;

            //    Byte.TryParse(PacketValue.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out HIDWriteInfo.ReportData[5]);
        }
        void SetByte6(object sender, TextChangedEventArgs e)
        {
            //    var PacketValue = sender as TextBox;

            //    Byte.TryParse(PacketValue.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out HIDWriteInfo.ReportData[6]);
        }
        void SetByte7(object sender, TextChangedEventArgs e)
        {
            //    var PacketValue = sender as TextBox;

            //    Byte.TryParse(PacketValue.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out HIDWriteInfo.ReportData[7]);
        }

        void HIDRead_Button_Click(object sender, RoutedEventArgs e)
        {
            //    HIDReadInfo.Active = false;

            //    UInt16.TryParse(ReadHID_VID_Input.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out HIDReadInfo.VendorID);
            //    UInt16.TryParse(ReadHID_PID_Input.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out HIDReadInfo.ProductID);

            //    CheckHIDRead();

            //    if (HIDReadInfo.Active == true)
            //    {
            //        var Device = HIDReadInfo.Device[HIDReadInfo.iDevice];

            //        ManufacturerName.Text = "Manufacturer: " + Device.Manufacturer;
            //        ProductName.Text      = "Product: "      + Device.Product;
            //        SerialNumber.Text     = "SerialNumber: " + Device.SerialNumber.ToString();
            //    }
        }
        void HIDWrite_Button_Click(object sender, RoutedEventArgs e)
        {
            //    HIDWriteInfo.Done   = false;
            //    HIDWriteInfo.Active = false;

            //    UInt16.TryParse(SendHID_VID_Input.Text      , NumberStyles.HexNumber, CultureInfo.InvariantCulture, out HIDWriteInfo.VendorID);
            //    UInt16.TryParse(SendHID_PID_Input.Text      , NumberStyles.HexNumber, CultureInfo.InvariantCulture, out HIDWriteInfo.ProductID);
            //    UInt16.TryParse(SendHID_UsagePage_Input.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out HIDWriteInfo.UsagePage);
            //    UInt16.TryParse(SendHID_Usage_Input.Text    , NumberStyles.HexNumber, CultureInfo.InvariantCulture, out HIDWriteInfo.Usage);
            //    Byte.TryParse(SendHID_RID_Input.Text        , NumberStyles.HexNumber, CultureInfo.InvariantCulture, out HIDWriteInfo.ReportID);

            //    CheckHIDWrite();
        }
        //==========================================================================================================================================================================================================
        // END_FUNCTION      END_FUNCTION      END_FUNCTION      END_FUNCTION      END_FUNCTION      END_FUNCTION      END_FUNCTION      END_FUNCTION      END_FUNCTION      END_FUNCTION      END_FUNCTION
        //==========================================================================================================================================================================================================

    }
}
