namespace chat_server.WebSocketModule.Interfaces;

public interface IAsciiDecoder
{
    string GetString(byte[] bytes);
}