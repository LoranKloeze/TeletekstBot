using Microsoft.EntityFrameworkCore;
using NSubstitute;
using TeletekstBotHangfire.Data;
using TeletekstBotHangfire.Jobs;
using TeletekstBotHangfire.Models.Ef;
using TeletekstBotHangfire.Services;
using TeletekstBotHangfire.Services.Interfaces;

namespace TheTests.Jobs;

[TestFixture]
public class PostNewPagesJobTests
{
    private ApplicationDbContext _context;
    private ITeletekstPageService _teletekstPageService;
    private ICurrentPagesService _currentPagesService;
    private IBlueSkyPostsService _blueSkyPostsService;
    private PostNewPagesJob _job;

    [SetUp]
    public void SetUp()
    {
        // Set up the mocks
        _teletekstPageService = Substitute.For<ITeletekstPageService>();
        _currentPagesService = Substitute.For<ICurrentPagesService>();
        _blueSkyPostsService = Substitute.For<IBlueSkyPostsService>();

        // Set up the in-memory database context
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        _job = new PostNewPagesJob(_context, _teletekstPageService, _currentPagesService, _blueSkyPostsService);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task StartAsync_Should_Post_New_Page_If_Not_In_Database()
    {
        // Arrange
        var pageNumbers = new List<int> { 101 };
        var newPage = new TeletekstPage { PageNr = 101, Title = "New Page", Content = "Content", Screenshot = []};

        _currentPagesService.GetPageNumbersAsync().Returns(pageNumbers);
        _teletekstPageService.GetPageAsync(101).Returns(newPage);

        // Act
        await _job.StartAsync();

        // Assert
        await _blueSkyPostsService.Received(1).SendTeletekstPageAsync(newPage);

        var pageInDb = await _context.TeletekstPages.FindAsync(101);
        Assert.That(pageInDb, Is.Not.Null);
        Assert.That(pageInDb.Title, Is.EqualTo("New Page"));
    }

    [Test]
    public async Task StartAsync_Should_Update_Page_If_Content_Has_Changed()
    {
        // Arrange
        var existingPage = new TeletekstPage { PageNr = 101, Title = "Old Page", Content = "Old Content", 
            Screenshot = []};
        await _context.TeletekstPages.AddAsync(existingPage);
        await _context.SaveChangesAsync();

        var updatedPage = new TeletekstPage { PageNr = 101, Title = "Updated Page", Content = "Updated Content", 
            Screenshot = [] };
        _currentPagesService.GetPageNumbersAsync().Returns(new List<int> { 101 });
        _teletekstPageService.GetPageAsync(101).Returns(updatedPage);

        // Act
        await _job.StartAsync();

        // Assert
        await _blueSkyPostsService.Received(1).SendTeletekstPageAsync(updatedPage);

        var pageInDb = await _context.TeletekstPages.FindAsync(101);
        Assert.Multiple(() =>
        {
            Assert.That(pageInDb, Is.Not.Null);
            Assert.That(pageInDb!.Title, Is.EqualTo("Updated Page"));
            Assert.That(pageInDb.Content, Is.EqualTo("Updated Content"));
        });
    }

    [Test]
    public async Task StartAsync_Should_Not_Post_If_Page_Has_Not_Changed()
    {
        // Arrange
        var existingPage = new TeletekstPage { PageNr = 101, Title = "Same Page", Content = "Same Content", Screenshot = [] };
        await _context.TeletekstPages.AddAsync(existingPage);
        await _context.SaveChangesAsync();

        var samePage = new TeletekstPage { PageNr = 101, Title = "Same Page", Content = "Same Content", Screenshot = [] };
        _currentPagesService.GetPageNumbersAsync().Returns(new List<int> { 101 });
        _teletekstPageService.GetPageAsync(101).Returns(samePage);

        // Act
        await _job.StartAsync();

        // Assert
        await _blueSkyPostsService.DidNotReceive().SendTeletekstPageAsync(Arg.Any<TeletekstPage>());
    }
}