namespace chat_server.WebSocketModule.Interfaces;

public interface IAsciiEncoder
{
    byte[] GetBytes(string str);
}