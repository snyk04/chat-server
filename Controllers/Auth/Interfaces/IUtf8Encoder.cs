namespace chat_server.Controllers.Auth.Interfaces;

public interface IUtf8Encoder
{
    byte[] GetBytes(string str);
}