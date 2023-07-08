namespace chat_server.Controllers.Chat.Interfaces;

public interface IJsonConverter
{
    string ConvertToJson<T>(T data);
}