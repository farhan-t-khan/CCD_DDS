//==========================================================================================================================================================================================================
// HEADER      HEADER      HEADER      HEADER      HEADER      HEADER      HEADER      HEADER      HEADER      HEADER      HEADER      HEADER      HEADER      HEADER      HEADER      HEADER      HEADER
//==========================================================================================================================================================================================================
using System;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using static USBHID.Kernel32;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using CCD_DDS;

//==========================================================================================================================================================================================================
// END_HEADER      END_HEADER      END_HEADER      END_HEADER      END_HEADER      END_HEADER      END_HEADER      END_HEADER      END_HEADER      END_HEADER      END_HEADER      END_HEADER
//==========================================================================================================================================================================================================
namespace USBHID
{
    public partial class Core : Window
    {
        [StructLayout(LayoutKind.Sequential)]
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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]  public byte[] ms_sensor_part_number;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]  public byte[] co_sensor_part_number;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]  public byte[] o2_sensor_part_number;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]  public byte[] hs_sensor_part_number;
            public byte status;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 113)] public byte[] pad;
        };


        //==========================================================================================================================================================================================================
        // DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA      DATA
        //==========================================================================================================================================================================================================
        //static Timer ReadTimer;
        //static Timer WriteTimer;

        static IntPtr USBHandle;
        static Flash_Root DetectorFlashRoot = new Flash_Root();
        //==========================================================================================================================================================================================================
        // END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA      END_DATA
        //==========================================================================================================================================================================================================

        //==========================================================================================================================================================================================================
        // CODE      CODE      CODE      CODE      CODE      CODE      CODE      CODE      CODE      CODE      CODE      CODE      CODE      CODE      CODE      CODE      CODE      CODE      CODE      CODE
        //==========================================================================================================================================================================================================


        public static class SharedData
        {
            public static string[] ModelAndSerial { get; set; }
        }


        public Core()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Entry);

            // BW code

            int success;
            if (DetectRovExUSB())
                success = 1;
            else
                success = 0;

            byte[] buf = new byte[264];
            //USBSendPacket(new byte[] { 0x2a });
            USBSendPacket(new byte[] { 0x00 });
            var n = USBReceivePacket(ref buf,8);
            USBSendPacket(new byte[] { 0x10 });
            n = USBReceivePacket(ref buf,8);
            USBReadFlashBlock(0, ref buf);
            InitializeDetectorFlashRoot();

            //USBReadFlashRoot();
            string[] modelAndSerial = USBReadFlashRoot();
            SharedData.ModelAndSerial = modelAndSerial;
            string[] ArrayComPortsNames = null;
            SerialPort ComPort = new SerialPort();

            ArrayComPortsNames = SerialPort.GetPortNames();
            
            ComPort.BaudRate = 9600;
            ComPort.DataBits = 8;
            ComPort.Parity = Parity.None;
            ComPort.StopBits = StopBits.One;
            ComPort.Handshake = Handshake.None;
            ComPort.PortName = "COM4";
            ComPort.Open();
            //ComPort.Write("Hello World");
            while (ComPort.BytesToWrite > 0) ;

            n = (uint)ComPort.BytesToRead;
            n = (uint)ComPort.Read(buf,0,ComPort.BytesToRead);
            ComPort.Close();
        }

        // scans USB for HID devices and sets USBHandle if it detectors "Bascom Turner", "Gas-Guardian"; if no detection then USBHandle = IntPtr.Zero
        public bool DetectRovExUSB()
        {
            // Initialize handle to zero, so we can check if we found RovEx or not
            USBHandle = IntPtr.Zero;

            // find out how many HID devices are connected
            int i = DeviceDiscovery.FindDeviceNumber();   
            
            // get info on all HID devices connected      
            HID_DEVICE[] HIDDeviceList = new HID_DEVICE[i];
            DeviceDiscovery.FindKnownHIDDevices(ref HIDDeviceList);

            // Check for "Bascom Turner" "Gas-Guardian"
            for (i = 0; i < HIDDeviceList.Length; i++)
            {
                if (HIDDeviceList[i].Manufacturer == "Bascom Turner" && HIDDeviceList[i].Product == "Gas-Guardian")
                {
                    USBHandle = HIDDeviceList[i].Handle;
                    return true;
                }
            }
            return false;
        }

        // if USB connected, calculates CRC and sends out 8 bytes as two 6 byte packets
        public bool USBSendPacket(byte[] pkt)
        {
            if (USBHandle == IntPtr.Zero)       // get out if no USBHandle
                return false;

            // calculate CRC
            byte[] buf = new byte[12];
            byte crc8 = 0;

            for (int x = 0; x < 11; x++)
            {
                if (x >= pkt.Length)
                    buf[x] = 0;
                else
                    buf[x] = pkt[x];
                crc8 = UpdateCRC8(buf[x], crc8);
            }
            buf[11] = crc8;

            // send out first half of packet
            byte[] outputbuf = new byte[9];
            outputbuf[0] = 0;
            Array.Copy(buf, 0, outputbuf, 1, 6);
            outputbuf[7] = 0;
            outputbuf[8] = 0;
            //for (int x = 0; x < 6; x++)
            //    outputbuf[x + 1] = buf[x];

            uint bytestransmitted = 0;
            Kernel32.WriteFile(USBHandle, outputbuf, 9, ref bytestransmitted, IntPtr.Zero);
            while (bytestransmitted < 9) ;
           

            // send out second half of packet
            outputbuf[0] = 0;
            Array.Copy(buf, 6, outputbuf, 1, 6);
            outputbuf[7] = 0;
            outputbuf[8] = 0;
            //for (int x = 0; x < 6; x++)
            //    outputbuf[x + 1] = buf[x + 6];

            Kernel32.WriteFile(USBHandle, outputbuf, 9, ref bytestransmitted, IntPtr.Zero);
            while (bytestransmitted < 9) ;
            return true;
        }

        // used to receive USB packets from USBRxThread
        static public byte[] USBInputBuffer = new Byte[512];
        static public uint USBInputBufferLength;

        // rxsize is number bytes expected, raw packet will be 9 bytes for every 8 bytes expected, plus a 9 byte header packet
        uint USBReceivePacket(ref byte[] buf,uint rxsize)
        {
            if (USBHandle == IntPtr.Zero)   // get out if no USB connection
                return 0;

            byte[] rxbuf = new byte[512];  // holds all bytes received
            uint rxbytes = 0;
            uint i;
        USBReceivePacket_Retry:
            USBInputBufferLength = 0;
            Thread USBRxThread = new Thread(new ThreadStart(ReadUSBThread));
            USBRxThread.Start();

            // 100ms timeout to receive data
            for(i = 0; i<10; i++)
            {
                Thread.Sleep(10);
                if (USBInputBufferLength > 0)
                    break;
            }
            if (USBRxThread.IsAlive)
                USBRxThread.Abort();

            // if we received nothing, we probably need to re-open handle to the detector
            if (USBInputBufferLength == 0)  
                DetectRovExUSB();
            else
            {
                Array.Copy(USBInputBuffer, 0, rxbuf, rxbytes, USBInputBufferLength);
                rxbytes += USBInputBufferLength;
                if(rxbytes < rxsize/8*9 +9)
                    goto USBReceivePacket_Retry;    // retry until we recieve 0 bytes
            }

            for(i = 0; i <rxsize/8; i++) // skip first 9 received bytes and strip off first byte from every 9 bytes.
            {
                Array.Copy(rxbuf, i * 9 + 9+1, buf, i * 8, 8);
            }
            
            return (rxbytes-9)*8/9 ;
        }

        static void ReadUSBThread()
        {
            Kernel32.ReadFile(USBHandle, USBInputBuffer, 297, ref USBInputBufferLength, IntPtr.Zero);
        }

        private byte[] TableCrc8 = new byte[256]
        {
        0x0, 0x5E, 0xBC, 0xE2, 0x61, 0x3F, 0xDD, 0x83, 0xC2, 0x9C, 0x7E, 0x20, 0xA3, 0xFD, 0x1F, 0x41,
        0x9D, 0xC3, 0x21, 0x7F, 0xFC, 0xA2, 0x40, 0x1E, 0x5F, 0x1, 0xE3, 0xBD, 0x3E, 0x60, 0x82, 0xDC,
        0x23, 0x7D, 0x9F, 0xC1, 0x42, 0x1C, 0xFE, 0xA0, 0xE1, 0xBF, 0x5D, 0x3, 0x80, 0xDE, 0x3C, 0x62,
        0xBE, 0xE0, 0x2, 0x5C, 0xDF, 0x81, 0x63, 0x3D, 0x7C, 0x22, 0xC0, 0x9E, 0x1D, 0x43, 0xA1, 0xFF,
        0x46, 0x18, 0xFA, 0xA4, 0x27, 0x79, 0x9B, 0xC5, 0x84, 0xDA, 0x38, 0x66, 0xE5, 0xBB, 0x59, 0x7,
        0xDB, 0x85, 0x67, 0x39, 0xBA, 0xE4, 0x6, 0x58, 0x19, 0x47, 0xA5, 0xFB, 0x78, 0x26, 0xC4, 0x9A,
        0x65, 0x3B, 0xD9, 0x87, 0x4, 0x5A, 0xB8, 0xE6, 0xA7, 0xF9, 0x1B, 0x45, 0xC6, 0x98, 0x7A, 0x24,
        0xF8, 0xA6, 0x44, 0x1A, 0x99, 0xC7, 0x25, 0x7B, 0x3A, 0x64, 0x86, 0xD8, 0x5B, 0x5, 0xE7, 0xB9,
        0x8C, 0xD2, 0x30, 0x6E, 0xED, 0xB3, 0x51, 0xF, 0x4E, 0x10, 0xF2, 0xAC, 0x2F, 0x71, 0x93, 0xCD,
        0x11, 0x4F, 0xAD, 0xF3, 0x70, 0x2E, 0xCC, 0x92, 0xD3, 0x8D, 0x6F, 0x31, 0xB2, 0xEC, 0xE, 0x50,
        0xAF, 0xF1, 0x13, 0x4D, 0xCE, 0x90, 0x72, 0x2C, 0x6D, 0x33, 0xD1, 0x8F, 0xC, 0x52, 0xB0, 0xEE,
        0x32, 0x6C, 0x8E, 0xD0, 0x53, 0xD, 0xEF, 0xB1, 0xF0, 0xAE, 0x4C, 0x12, 0x91, 0xCF, 0x2D, 0x73,
        0xCA, 0x94, 0x76, 0x28, 0xAB, 0xF5, 0x17, 0x49, 0x8, 0x56, 0xB4, 0xEA, 0x69, 0x37, 0xD5, 0x8B,
        0x57, 0x9, 0xEB, 0xB5, 0x36, 0x68, 0x8A, 0xD4, 0x95, 0xCB, 0x29, 0x77, 0xF4, 0xAA, 0x48, 0x16,
        0xE9, 0xB7, 0x55, 0xB, 0x88, 0xD6, 0x34, 0x6A, 0x2B, 0x75, 0x97, 0xC9, 0x4A, 0x14, 0xF6, 0xA8,
        0x74, 0x2A, 0xC8, 0x96, 0x15, 0x4B, 0xA9, 0xF7, 0xB6, 0xE8, 0xA, 0x54, 0xD7, 0x89, 0x6B, 0x35
        };

        private byte UpdateCRC8(byte newbyte, byte existingCRC8)
        {
            return(TableCrc8[existingCRC8 ^ newbyte]);
        }

        // reads 264 block from flash chip
        bool USBReadFlashBlock(UInt32 bn, ref byte[] buf)
        {
            if (USBHandle == IntPtr.Zero)
                return false;
            byte b0 = (byte)(bn & 0xFF);
            byte b1 = (byte)((bn >> 8) & 0xFF);
            byte b2 = (byte)((bn >> 16) & 0xFF);
            byte b3 = (byte)((bn >> 24) & 0xFF);
            USBSendPacket(new byte[] { 0x01, b3, b2, b1, b0 });

            uint n = USBReceivePacket(ref buf,264) ;
            if (n == 264)
                return true;
            else
                return false;
        }

        string[] USBReadFlashRoot()
        {
            string[] arr = new string[2];
            byte[] tempbuf = new byte[512];
            if (USBReadFlashBlock(0, ref tempbuf) == false)
                //return false;
                return arr;
            DetectorFlashRoot.type = tempbuf[0];
            if (BitConverter.IsLittleEndian)
                Array.Reverse(tempbuf, 1, 4);
            DetectorFlashRoot.directory_start_page = BitConverter.ToUInt32(tempbuf, 1);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(tempbuf, 5, 4);
            DetectorFlashRoot.pages_in_directory = BitConverter.ToUInt32(tempbuf,5);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(tempbuf, 9, 2);
            DetectorFlashRoot.size_of_pages = BitConverter.ToUInt16(tempbuf, 9);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(tempbuf, 11, 4);
            DetectorFlashRoot.pages_in_memory = BitConverter.ToUInt32(tempbuf, 11);          
            Array.Copy(tempbuf, 15, DetectorFlashRoot.unit_serial_number, 0, 16);      
            Array.Copy(tempbuf, 31, DetectorFlashRoot.unit_model_number, 0, 11);    
            Array.Copy(tempbuf, 42, DetectorFlashRoot.CPU_ID, 0, 16);       
            Array.Copy(tempbuf, 58, DetectorFlashRoot.data_base_revision, 0, 16);        
            Array.Copy(tempbuf, 74, DetectorFlashRoot.instrument_name, 0, 32);    
            Array.Copy(tempbuf, 106, DetectorFlashRoot.revision_of_instrument_software, 0, 16);        
            Array.Copy(tempbuf, 122, DetectorFlashRoot.ms_sensor_part_number, 0, 7);
            Array.Copy(tempbuf, 129, DetectorFlashRoot.co_sensor_part_number, 0, 7);
            Array.Copy(tempbuf, 136, DetectorFlashRoot.o2_sensor_part_number, 0, 7);
            Array.Copy(tempbuf, 143, DetectorFlashRoot.hs_sensor_part_number, 0, 7);
            DetectorFlashRoot.status = tempbuf[150];
            Array.Copy(tempbuf, 151, DetectorFlashRoot.pad, 0, 113);
            
            //Write out in a human readable format
            string serialNumber = Encoding.ASCII.GetString(DetectorFlashRoot.unit_serial_number);
            //Console.WriteLine("Serial Number: " + serialNumber);

            string modelNumber = Encoding.ASCII.GetString(DetectorFlashRoot.unit_model_number);
            //Console.WriteLine("Model Number: " + modelNumber);
            
            
            arr[0] = modelNumber;
            arr[1] = serialNumber;
            //return true;
            return arr;
        }

        void InitializeDetectorFlashRoot()
        {
            DetectorFlashRoot.unit_serial_number = new byte[16];
            DetectorFlashRoot.unit_model_number = new byte[11];
            DetectorFlashRoot.CPU_ID = new byte[16];
            DetectorFlashRoot.data_base_revision = new byte[16];
            DetectorFlashRoot.instrument_name = new byte[32];
            DetectorFlashRoot.revision_of_instrument_software = new byte[16];
            DetectorFlashRoot.ms_sensor_part_number = new byte[7];
            DetectorFlashRoot.co_sensor_part_number = new byte[7];
            DetectorFlashRoot.o2_sensor_part_number = new byte[7];
            DetectorFlashRoot.hs_sensor_part_number = new byte[7];
            DetectorFlashRoot.pad = new byte[113];
         }

        public void CalibrationSuccessful()
        {
            byte[] buf = new byte[264];
            USBSendPacket(new byte[] { 0x2a });
            var n = USBReceivePacket(ref buf, 8);
            USBSendPacket(new byte[] { 0x10 });
            n = USBReceivePacket(ref buf, 8);
            USBReadFlashBlock(0, ref buf);
        }

        public void PumpOn()
        {
            byte[] buf = new byte[264];
            USBSendPacket(new byte[] { 0x20 });
            var n = USBReceivePacket(ref buf, 8);
        }

        public void PumpOff()
        {
            byte[] buf = new byte[264];
            USBSendPacket(new byte[] { 0x21 });
            var n = USBReceivePacket(ref buf, 8);
        }

        void    Entry(object sender, EventArgs Data)
        {
            //HIDReadInfo.ReportData = new List<Byte>();

            //ReadTimer  = new Timer(BeginAsyncRead, null, TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(1));
            //WriteTimer = new Timer(BeginSyncSend , null, TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(1));

            //CompositionTarget.Rendering += OnRendering;
        }
        //==========================================================================================================================================================================================================
        // END_CODE      END_CODE      END_CODE      END_CODE      END_CODE      END_CODE      END_CODE      END_CODE      END_CODE      END_CODE      END_CODE      END_CODE      END_CODE      END_CODE
        //==========================================================================================================================================================================================================

        //==========================================================================================================================================================================================================
        // FUNCTION      FUNCTION      FUNCTION      FUNCTION      FUNCTION      FUNCTION      FUNCTION      FUNCTION      FUNCTION      FUNCTION      FUNCTION      FUNCTION      FUNCTION      FUNCTION
        //==========================================================================================================================================================================================================
        //void    OnRendering(object sender, EventArgs e)
        //{
        //    if (HIDReadInfo.Active == true)
        //    {
        //        Read_Output.Clear();

        //        for (var Index = 0; Index < HIDReadInfo.ReportData.Count; Index++)
        //        {
        //            Read_Output.Text += HIDReadInfo.ReportData[Index].ToString("X2");
        //            Read_Output.Text += " - ";
        //        }
        //    }
        //}
        //void    CheckHIDRead()
        //{
        //    HIDReadInfo.Device = new HID_DEVICE[DeviceDiscovery.FindDeviceNumber()];

        //    DeviceDiscovery.FindKnownHIDDevices(ref HIDReadInfo.Device);

        //    for (var Index = 0; Index < HIDReadInfo.Device.Length; Index++)
        //    {
        //        if (HIDReadInfo.VendorID != 0)
        //        {
        //            var Count = 0;

        //            if (HIDReadInfo.Device[Index].Attributes.VendorID == HIDReadInfo.VendorID)
        //            {
        //                Count++;
        //            }
        //            if (HIDReadInfo.Device[Index].Attributes.ProductID == HIDReadInfo.ProductID)
        //            {
        //                Count++;
        //            }

        //            if (Count == 2)
        //            {
        //                HIDReadInfo.iDevice = Index;
        //                HIDReadInfo.Active  = true;

        //                return;
        //            }
        //        }
        //    }
        //}
        //void    CheckHIDWrite()
        //{
        //    HIDWriteInfo.Device = new HID_DEVICE[DeviceDiscovery.FindDeviceNumber()];

        //    DeviceDiscovery.FindKnownHIDDevices(ref HIDWriteInfo.Device);

        //    for (var Index = 0; Index < HIDWriteInfo.Device.Length; Index++)
        //    {
        //        if (HIDWriteInfo.VendorID != 0)
        //        {
        //            var Count = 0;

        //            if (HIDWriteInfo.Device[Index].Attributes.VendorID == HIDWriteInfo.VendorID)
        //            {
        //                Count++;
        //            }
        //            if (HIDWriteInfo.Device[Index].Attributes.ProductID == HIDWriteInfo.ProductID)
        //            {
        //                Count++;
        //            }
        //            if (HIDWriteInfo.Device[Index].Caps.UsagePage == HIDWriteInfo.UsagePage)
        //            {
        //                Count++;
        //            }
        //            if (HIDWriteInfo.Device[Index].Caps.Usage == HIDWriteInfo.Usage)
        //            {
        //                Count++;
        //            }

        //            if (Count == 4)
        //            {
        //                HIDWriteInfo.iDevice = Index;
        //                HIDWriteInfo.Active  = true;

        //                return;
        //            }
        //        }
        //    }
        //}

        void    SetByte0(object sender, TextChangedEventArgs e)
        {
        //    var PacketValue = sender as TextBox;

        //    Byte.TryParse(PacketValue.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out HIDWriteInfo.ReportData[0]);
        }
        void    SetByte1(object sender, TextChangedEventArgs e)
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
            void    HIDWrite_Button_Click(object sender, RoutedEventArgs e)
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
