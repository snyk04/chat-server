using chat_server.Base;

namespace Tests.Base;

[TestFixture]
public class GuidGeneratorTests
{
    private GuidGenerator guidGenerator;

    [SetUp]
    public void SetUp()
    {
        guidGenerator = new GuidGenerator();
    }
    
    [Test]
    public void GetNewGuid_ReturnsUniqueGuid()
    {
        var guid1 = guidGenerator.GetNewGuid();
        var guid2 = guidGenerator.GetNewGuid();
        
        Assert.That(guid2, Is.Not.EqualTo(guid1));
    }
}