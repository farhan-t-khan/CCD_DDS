using System;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
//using static USBHID.Core;
using USBHID;

namespace USBHID
{
    public class SharedData
    {
        public string[]? ModelAndSerial { get; set; }
    }
    public class USBCommunication : ICommunication, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string? _modelNumber;
        public string? Model
        {
            get => _modelNumber;
            set
            {
                _modelNumber = value;
                OnPropertyChanged(nameof(Model));
            }
        }

        private string? _serial;
        public string? Serial
        {
            get => _serial;
            set
            {
                _serial = value;
                OnPropertyChanged(nameof(Serial));
            }
        }
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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public byte[] ms_sensor_part_number;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public byte[] co_sensor_part_number;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public byte[] o2_sensor_part_number;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public byte[] hs_sensor_part_number;
            public byte status;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 113)] public byte[] pad;
        }


        static IntPtr USBHandle;
        static Flash_Root DetectorFlashRoot = new Flash_Root();

        public IntPtr GetUSBHandle()
        {
            return USBHandle;
        }

        public bool DetectRovEx()
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

        public bool SendPacket(byte[] pkt)
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
        /*        public uint ReceivePacket(ref byte[] buf, uint rxsize)
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
                    for (i = 0; i < 10; i++)
                    {
                        Thread.Sleep(10);
                        if (USBInputBufferLength > 0)
                            break;
                    }
                    if (USBRxThread.IsAlive)
                        USBRxThread.Abort();

                    // if we received nothing, we probably need to re-open handle to the detector
                    if (USBInputBufferLength == 0)
                        DetectRovEx();
                    else
                    {
                        Array.Copy(USBInputBuffer, 0, rxbuf, rxbytes, USBInputBufferLength);
                        rxbytes += USBInputBufferLength;
                        if (rxbytes < rxsize / 8 * 9 + 9)
                            goto USBReceivePacket_Retry;    // retry until we recieve 0 bytes
                    }

                    for (i = 0; i < rxsize / 8; i++) // skip first 9 received bytes and strip off first byte from every 9 bytes.
                    {
                        Array.Copy(rxbuf, i * 9 + 9 + 1, buf, i * 8, 8);
                    }

                    return (rxbytes - 9) * 8 / 9;
                }*/

        public uint ReceivePacket(ref byte[] buf, uint rxsize)
        {
            if (USBHandle == IntPtr.Zero)   // get out if no USB connection
                return 0;

            byte[] rxbuf = new byte[512];  // holds all bytes received
            uint rxbytes = 0;
            uint i;

        USBReceivePacket_Retry:
            USBInputBufferLength = 0;

            CancellationTokenSource cts = new CancellationTokenSource();
            Thread USBRxThread = new Thread(new ThreadStart(ReadUSBThread));
            USBRxThread.Start();

            // 100ms timeout to receive data
            for (i = 0; i < 10; i++)
            {
                Thread.Sleep(10);
                if (USBInputBufferLength > 0)
                    break;
            }
            USBRxThread = null;
            cts.Cancel();  // Signal the thread to stop
            //USBRxThread.Join();  // Wait for the thread to complete

            // if we received nothing, we probably need to re-open handle to the detector
            if (USBInputBufferLength == 0)
                DetectRovEx();
            else
            {
                Array.Copy(USBInputBuffer, 0, rxbuf, rxbytes, USBInputBufferLength);
                rxbytes += USBInputBufferLength;
                if (rxbytes < rxsize / 8 * 9 + 9)
                    goto USBReceivePacket_Retry;    // retry until we receive 0 bytes
            }

            for (i = 0; i < rxsize / 8; i++) // skip first 9 received bytes and strip off first byte from every 9 bytes.
            {
                Array.Copy(rxbuf, i * 9 + 9 + 1, buf, i * 8, 8);
            }

            return (rxbytes - 9) * 8 / 9;
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
            return (TableCrc8[existingCRC8 ^ newbyte]);
        }

        public bool ReadFlashBlock(uint bn, ref byte[] buf)
        {
            if (USBHandle == IntPtr.Zero)
                return false;
            byte b0 = (byte)(bn & 0xFF);
            byte b1 = (byte)((bn >> 8) & 0xFF);
            byte b2 = (byte)((bn >> 16) & 0xFF);
            byte b3 = (byte)((bn >> 24) & 0xFF);
            SendPacket(new byte[] { 0x01, b3, b2, b1, b0 });

            uint n = ReceivePacket(ref buf, 264);
            if (n == 264)
                return true;
            else
                return false;
        }
        bool ReadFlashRoot()
        {
            byte[] tempbuf = new byte[512];
            if (ReadFlashBlock(0, ref tempbuf) == false)
                return false;
            DetectorFlashRoot.type = tempbuf[0];
            if (BitConverter.IsLittleEndian)
                Array.Reverse(tempbuf, 1, 4);
            DetectorFlashRoot.directory_start_page = BitConverter.ToUInt32(tempbuf, 1);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(tempbuf, 5, 4);
            DetectorFlashRoot.pages_in_directory = BitConverter.ToUInt32(tempbuf, 5);
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

            SharedData[] modelAndSerial = new SharedData[2];
            //Write out in a human readable format
            string serialNumber = Encoding.ASCII.GetString(DetectorFlashRoot.unit_serial_number);

            Serial = serialNumber;
            //Console.WriteLine("Serial Number: " + serialNumber);


            string modelNumber = Encoding.ASCII.GetString(DetectorFlashRoot.unit_model_number);

            Model = modelNumber;
            //Console.WriteLine("Model Number: " + modelNumber);


            return true;
        }

        /*        public void InitializeDetectorFlashRoot(byte[] buf)
                {
                    // Initialize DetectorFlashRoot with data from buf
                }*/

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

        public void Setup()
        {
            byte[] buf = new byte[264];
            SendPacket(new byte[] { 0x00 });
            var n = ReceivePacket(ref buf, 8);
            SendPacket(new byte[] { 0x10 });
            n = ReceivePacket(ref buf, 8);
            ReadFlashBlock(0, ref buf);
            InitializeDetectorFlashRoot();
            ReadFlashRoot();

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
            n = (uint)ComPort.Read(buf, 0, ComPort.BytesToRead);
            ComPort.Close();
        }

        public void pumpOn()
        {
            byte[] buf = new byte[264];
            SendPacket(new byte[] { 0x20 });
            var n = ReceivePacket(ref buf, 8);
        }
        public void pumpOff()
        {
            byte[] buf = new byte[264];
            SendPacket(new byte[] { 0x21 });
            var n = ReceivePacket(ref buf, 8);
        }

        /*        public string readCalibrationResults()
                {
                    byte[] buf = new byte[264];
                    SendPacket(new byte[] { 0x17 });
                    var n = ReceivePacket(ref buf, 8);
                    return n.ToString();
                }*/

        /*        public string readCalibrationResults()
                {
                    byte[] buf = new byte[264];
                    byte[] pageAddressBytes = new byte[4];
                    int pageAddress = 0;

                    // Step 1: Send the 0x17 command to get the calibration start page address
                    SendPacket(new byte[] { 0x17 });
                    var n = ReceivePacket(ref buf, 8);

                    // Check if the response is valid
                    if (n >= 8 && buf[0] == 0x17 && buf[1] == 0x06)
                    {
                        // Extract the page address from the response (last 4 bytes)
                        pageAddressBytes = new byte[] { buf[4], buf[5], buf[6], buf[7] };
                        pageAddress = BitConverter.ToInt32(pageAddressBytes, 0);

                        // Step 2: Read the calibration data from the starting page address
                        SendPacket(new byte[] { 0x01 }
                            .Concat(BitConverter.GetBytes(pageAddress))
                            .ToArray());
                        var dataLength = ReceivePacket(ref buf, 264);

                        // Check if the response is valid
                        if (dataLength >= 264 && buf[0] == 0x01 && buf[1] == 0x06)
                        {
                            // Convert data to string for demonstration (adjust according to your needs)
                            return BitConverter.ToString(buf.Skip(2).ToArray()).Replace("-", " ");
                        }
                        else
                        {
                            return "Error reading calibration data. Start Address was: " + n + "\n"
                                + "buf[0] = " + buf[0] + ", buf[1] = " + buf[1] + "\n"
                                + BitConverter.ToString(buf.Skip(2).ToArray()).Replace("-", " ");
                        }
                    }
                    else
                    {
                        return "Error retrieving calibration start page address";
                    }
                }*/

        public string readCalibrationResults()
        {
            byte[] buf = new byte[264]; // Assuming a page size of 264 bytes
            int rootPageAddress = 0x00000000; // Replace with actual root page address
            int callogDbStartPage = 0;
            uint callogDbSize = 0;

            using (StreamWriter writer = new StreamWriter("CallogDbLog.txt", true))
            {
                try
                {
                    // Step 1: Read the root directory page
                    SendPacket(new byte[] { 0x01 }.Concat(BitConverter.GetBytes(rootPageAddress)).ToArray());
                    uint n = ReceivePacket(ref buf, 264);

                    if (n >= 264 && buf[0] == 0x01)
                    {
                        // Step 2: Parse the directory entries
                        for (int i = 0; i < 264; i += 28)
                        {
                            //parsing of the directory entry
                            byte fileType = buf[i];
                            int drid = BitConverter.ToInt16(buf, i + 2);
                            int startPage = BitConverter.ToInt32(buf, i + 10);
                            uint sizeInPages = (uint)BitConverter.ToInt32(buf, i + 14);

                            if (drid == 0x15) // Directory ID of callog.db
                            {
                                callogDbStartPage = startPage;
                                callogDbSize = sizeInPages;
                                break;
                            }
                        }

                        if (callogDbStartPage > 0 && callogDbSize > 0)
                        {
                            // Step 3: Read the callog.db file
                            SendPacket(new byte[] { 0x01 }.Concat(BitConverter.GetBytes(callogDbStartPage)).ToArray());
                            uint dataLength = ReceivePacket(ref buf, 264 * callogDbSize);

                            if (dataLength >= callogDbSize * 264)
                            {
                                // Process the read data
                                return BitConverter.ToString(buf).Replace("-", " ");
                            }
                            else
                            {
                                writer.WriteLine("Error reading callog.db data.");
                                return "Error reading callog.db data.";
                            }
                        }
                        else
                        {
                            writer.WriteLine("callog.db not found in directory.");
                            return "callog.db not found in directory.";
                        }
                    }
                    else
                    {
                        writer.WriteLine("Error reading root directory page.");
                        return "Error reading root directory page.";
                    }
                }
                catch (Exception ex)
                {
                    writer.WriteLine($"Exception: {ex.Message}");
                    return "Error occurred during callog.db read.";
                }
            }
        }
    }
}
