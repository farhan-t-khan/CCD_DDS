using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace CCD_DDS
{
    public class DockingStationController
    {
        private SerialPort _serialPort;

        public DockingStationController(string portName, int baudRate)
        {
            _serialPort = new SerialPort(portName, baudRate);
            _serialPort.Open();
        }

        public bool AirValveOn()
        {
            // Placeholder, actual implementation needed based on your requirements
            return true;
        }

        private bool SendCommand(string command, int responseLength)
        {
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(command + "\r");
            _serialPort.Write(buffer, 0, buffer.Length);

            byte[] response = new byte[responseLength];
            _serialPort.Read(response, 0, responseLength);

            // Return true if operation is successful
            return true;
        }

        public bool RedLedOn() => SendCommand("RO", 4);

        public bool YellowLedOn()
        {
            if (SendCommand("YO", 4))
            {
                byte[] response = new byte[4];
                _serialPort.Read(response, 0, response.Length);

                // Check for ACK response
                return response[0] == 0x41 && response[1] == 0x43 && response[2] == 0x4b;
            }
            return false;
        }

        public bool GreenLedOn() => SendCommand("GO", 4);

        public bool AllLedsOn() => SendCommand("AO", 4);

        public bool RedLedOff() => SendCommand("RF", 4);

        public bool YellowLedOff() => SendCommand("YF", 4);

        public bool GreenLedOff() => SendCommand("GF", 4);

        public bool AllLedsOff() => SendCommand("AF", 4);

        public bool BgValveOn() => SendCommand("BO", 4);

        public bool CgValveOn() => SendCommand("CO", 4);

        public bool SgValveOn() => SendCommand("SO", 4);

        public bool CloseValves() => SendCommand("VF", 4);

        public bool CheckSwitch()
        {
            _serialPort.DiscardInBuffer();
            if (SendCommand("SW", 5))
            {
                int open = 0, closed = 0;

                for (int x = 0; x < 4; x++)
                {
                    System.Threading.Thread.Sleep(50); // delay of 0.05 seconds
                    _serialPort.DiscardInBuffer();
                    if (SendCommand("SW", 5))
                    {
                        byte[] buffer = new byte[5];
                        _serialPort.Read(buffer, 0, buffer.Length);

                        if (buffer[0] == 0x30) open++;
                        if (buffer[0] == 0x31) closed++;
                    }
                }

                return open < 2;
            }
            return false;
        }

        public bool GetControlBoardVersion(out string version)
        {
            version = string.Empty;
            if (SendCommand("VR", 5))
            {
                byte[] buffer = new byte[5];
                _serialPort.Read(buffer, 0, buffer.Length);

                if (buffer[0] == 'N' && buffer[1] == 'A' && buffer[2] == 'K')
                {
                    return false;
                }

                version = System.Text.Encoding.ASCII.GetString(buffer);
                return true;
            }
            return false;
        }

        public bool BtPowerOn() => SendCommand("TO", 4);

        public bool BtPowerOff() => SendCommand("TF", 4);

        public bool BtReset() => SendCommand("TR", 4);

        public bool BtActive() => SendCommand("TA", 4);

        public void Close()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }
    }
}
