namespace USBHID
{
    public interface ICommunication
    {
        bool DetectRovEx();
        bool SendPacket(byte[] pkt);
        uint ReceivePacket(ref byte[] buf, uint size);
        bool ReadFlashBlock(uint blockNumber, ref byte[] buf);
    }
}
