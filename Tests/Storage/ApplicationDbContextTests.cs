using chat_server.Storage;
using Microsoft.EntityFrameworkCore;

namespace Tests.Storage;

[TestFixture]
public class ApplicationDbContextTests
{
    private DbContextOptions<ApplicationDbContext> options;

    [SetUp]
    public void Setup()
    {
        options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    }

    [Test]
    public void Constructor_WhenOptionsProvided_EnsuresDatabaseCreated()
    {
        using var dbContext = new ApplicationDbContext(options);
        Assert.IsFalse(dbContext.Database.EnsureCreated());
    }

    [Test]
    public void Constructor_WhenOptionsNotProvided_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var _ = new ApplicationDbContext(null);
        });
    }
}