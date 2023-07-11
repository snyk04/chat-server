using System.Text;
using chat_server.WebSocketModule.Interfaces;

namespace chat_server.Base;

public class AsciiDecoder : IAsciiDecoder
{
    public string GetString(byte[] bytes)
    {
        return Encoding.ASCII.GetString(bytes);
    }
}