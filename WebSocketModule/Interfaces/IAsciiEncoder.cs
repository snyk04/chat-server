namespace chat_server.WebSocketModule.Interfaces;

public interface IAsciiEncoder
{
    string GetString(byte[] bytes, int index, int count);
    byte[] GetBytes(string str);
}