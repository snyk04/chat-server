using chat_server.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tests.Base;

[TestFixture]
public class NewtonsoftJsonConverterTests
{
    private NewtonsoftJsonConverter jsonConverter;

    [SetUp]
    public void SetUp()
    {
        jsonConverter = new NewtonsoftJsonConverter();
    }
    
    [Test]
    public void ConvertToJson_WhenObjectIsNotNull_ReturnsValidJsonString()
    {
        var data = new { Name = "John", Age = 30 };

        var json = jsonConverter.ConvertToJson(data);

        Assert.That(IsValidJson(json), Is.True);
    }

    [Test]
    public void ConvertToJson_WhenObjectIsNull_ReturnsEmptyJsonString()
    {
        var json = jsonConverter.ConvertToJson<object>(null);

        Assert.That(json, Is.EqualTo("null"));
    }

    private bool IsValidJson(string json)
    {
        try
        {
            JToken.Parse(json);
            return true;
        }
        catch (JsonReaderException)
        {
            return false;
        }
    }
}