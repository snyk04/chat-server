using System.Text;
using chat_server.Base;

namespace Tests.Base;

[TestFixture]
public class AsciiEncoderTests
{
    private AsciiEncoder asciiEncoder;

    [SetUp]
    public void Setup()
    {
        asciiEncoder = new AsciiEncoder();
    }

    [Test]
    public void GetBytes_StringIsOk_ReturnsEncodedValue()
    {
        const string testString = "Hello, World!";

        var actual = asciiEncoder.GetBytes(testString);

        var expected = Encoding.ASCII.GetBytes(testString);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetBytes_StringIsNull_ThrowsArgumentNullException()
    {
        var testDelegate = new TestDelegate(() => { asciiEncoder.GetBytes(null); });
        Assert.Throws(typeof(ArgumentNullException), testDelegate);
    }
    
    [Test]
    public void GetBytes_StringContainsNonAsciiCharacters_ReturnsEncodedBytesWithoutInvalidCharacters()
    {
        const string testString = "Hello, 世界!";

        var result = asciiEncoder.GetBytes(testString);

        var expectedBytes = "Hello, ??!"u8.ToArray();
        Assert.That(result, Is.EqualTo(expectedBytes));
    }
}