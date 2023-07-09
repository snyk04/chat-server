using System.Text;
using chat_server.Controllers.Auth.Interfaces;

namespace chat_server.Base;

public class Utf8Encoder : IUtf8Encoder
{
    public byte[] GetBytes(string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }
}