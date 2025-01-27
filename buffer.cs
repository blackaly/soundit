using System.Text;

namespace Bufferprotocol;

internal class BufferProtocol
{
    private readonly MemoryStream buffer = new ();

    private void WriteVarint(long val)
    {
        do
        {
            byte b = (byte)(val & 0X7F);
            val >>= 7; // Extracting the remaining 7 bits
            if(val != 0)
                b |= 0x80;
            buffer.WriteByte(b);
        }while(val != 0);
    }

    // Creating the header
    private void WriteField(int field, byte wire)
    {
        long fieldIntoBytes = (long)field << 3;
        long wireIntoBytes = (long)wire & 0x07;
        long val = fieldIntoBytes | wireIntoBytes;

        WriteVarint(val);
    }

    public BufferProtocol AddNumber(int field, long val)
    {
        WriteField(field, 0);
        WriteVarint(val);
        return this;
    }
    public BufferProtocol AddBytes(int field, byte[] bytes)
    {
        WriteField(field, 2);
        WriteVarint(bytes.Length);
        buffer.Write(bytes, 0, bytes.Length);
        return this;
    }

    public BufferProtocol AddString(int field, string str)
    {
        AddBytes(field, Encoding.UTF8.GetBytes(str));
        return this;
    }

    public byte[] ToBytesArray() => buffer.ToArray();
}