using System.Text;
using chat_server.WebSocketModule.Interfaces;

namespace chat_server.Base;

public class AsciiEncoder : IEncoder
{
    public string GetString(byte[] bytes, int index, int count)
    {
        return Encoding.ASCII.GetString(bytes, index, count);
    }

    public byte[] GetBytes(string str)
    {
        return Encoding.ASCII.GetBytes(str);
    }
}