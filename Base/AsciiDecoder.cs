using System.Text;
using chat_server.WebSocketModule.Interfaces;

namespace chat_server.Base;

public class AsciiDecoder : IAsciiDecoder
{
    public string GetString(byte[] bytes, int index, int count)
    {
        return Encoding.ASCII.GetString(bytes, index, count);
    }
}