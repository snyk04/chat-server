using System.Text;
using chat_server.WebSocketModule.Interfaces;

namespace chat_server.Base;

public class AsciiEncoder : IAsciiEncoder
{
    public byte[] GetBytes(string str)
    {
        return Encoding.ASCII.GetBytes(str);
    }
}