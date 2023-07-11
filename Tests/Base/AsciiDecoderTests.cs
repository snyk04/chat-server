using System.Text;
using chat_server.Base;

namespace Tests.Base;

[TestFixture]
public class AsciiDecoderTests
{
    private AsciiDecoder asciiDecoder;

    [SetUp]
    public void SetUp()
    {
        asciiDecoder = new AsciiDecoder();
    }
    
    [Test]
    public void GetString_BytesAreOk_ReturnsDecodedValue()
    {
        const string str = "Hello, World!";
        var bytes = Encoding.ASCII.GetBytes(str);

        var result = asciiDecoder.GetString(bytes);

        Assert.That(result, Is.EqualTo(str));
    }
    
    [Test]
    public void GetString_BytesAreNull_ThrowsArgumentNullException()
    {
        var testDelegate = new TestDelegate(() => { asciiDecoder.GetString(null); });
        Assert.Throws(typeof(ArgumentNullException), testDelegate);
    }
    
    [Test]
    public void GetString_BytesContainsNonAsciiCharacters_ReturnsDecodedStringWithInvalidCharacters()
    {
        var bytes = new byte[] { 72, 101, 108, 108, 111, 44, 32, 128, 87, 111, 114, 108, 100, 33 };

        var result = asciiDecoder.GetString(bytes);

        Assert.That(result, Is.EqualTo("Hello, ?World!"));
    }
}