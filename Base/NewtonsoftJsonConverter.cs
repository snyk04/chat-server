using chat_server.Controllers.Chat.Interfaces;
using Newtonsoft.Json;

namespace chat_server.Base;

public class NewtonsoftJsonConverter : IJsonConverter
{
    public string ConvertToJson<T>(T data)
    {
        return JsonConvert.SerializeObject(data);
    }
}