using System.Text;
using chat_server.Base;

namespace Tests.Base;

[TestFixture]
public class Utf8EncoderTests
{
    private Utf8Encoder utf8Encoder;

    [SetUp]
    public void Setup()
    {
        utf8Encoder = new Utf8Encoder();
    }

    [Test]
    public void GetBytes_StringIsOk_ReturnsEncodedValue()
    {
        const string testString = "Hello, World!";

        var actual = utf8Encoder.GetBytes(testString);

        var expected = Encoding.UTF8.GetBytes(testString);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetBytes_StringIsNull_ThrowsArgumentNullException()
    {
        var testDelegate = new TestDelegate(() => { utf8Encoder.GetBytes(null); });
        Assert.Throws(typeof(ArgumentNullException), testDelegate);
    }
}