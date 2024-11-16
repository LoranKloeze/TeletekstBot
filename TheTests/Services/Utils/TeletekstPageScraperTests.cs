using Microsoft.Playwright;
using NSubstitute;
using TeletekstBotHangfire.Services.Utils;

namespace TheTests.Services.Utils;

[TestFixture]
public class TeletekstPageScraperTests
{
    private IPage _browserPageMock;
    private TeletekstPageScraper _scraper;

    [SetUp]
    public void Setup()
    {
        _browserPageMock = Substitute.For<IPage>();
        _scraper = new TeletekstPageScraper();
    }

    [Test]
    public async Task RetrievePageAsync_ShouldReturnTeletekstPage_WithCorrectTitleAndContent()
    {
        // Arrange
        const int pageNr = 101;
        const string expectedTitle = "I am a title";
        const string expectedContent = "This is the content of the page.And some more.And more.";

        var mockTextBlocks = new List<string>
        {
            "", "", "", "", "I am a title", // \
            "", "This is the content of the page.", "And some more.", "And more.", 
            "No content", "No", "No", "No"
        };

        MockTextBlocksAsync(mockTextBlocks);

        // Act
        var result = await _scraper.RetrievePageAsync(_browserPageMock, pageNr);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Title, Is.EqualTo(expectedTitle));
            Assert.That(result.Content, Is.EqualTo(expectedContent));
            Assert.That(result.PageNr, Is.EqualTo(pageNr));
        });
    }
        
    [Test]
    public async Task RetrievePageAsyncWithSpecialChars_ShouldReturnTeletekstPage_WithCorrectTitleAndContent()
    {
        // Arrange
        const int pageNr = 101;
        const string expectedTitle = "I am a title";
        const string expectedContent = "This is the content of the page.And some more.And more.";

        var mockTextBlocks = new List<string>
        {
            "", "", "", "", "I am a =title", // \
            "", "This    is the content of the page.", "And some more.", "And more.", 
            "No content", "No", "No", "No"
        };

        MockTextBlocksAsync(mockTextBlocks);

        // Act
        var result = await _scraper.RetrievePageAsync(_browserPageMock, pageNr);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Title, Is.EqualTo(expectedTitle));
            Assert.That(result.Content, Is.EqualTo(expectedContent));
            Assert.That(result.PageNr, Is.EqualTo(pageNr));
        });
    }

    

    private void MockTextBlocksAsync(List<string> blocks)
    {
        var preElementMock = Substitute.For<IElementHandle>();
        var spanElementsMock = blocks.Select(block =>
        {
            var spanMock = Substitute.For<IElementHandle>();
            spanMock.InnerTextAsync().Returns(block);
            return spanMock;
        }).ToList();

        _browserPageMock.QuerySelectorAllAsync("pre").Returns(new List<IElementHandle> { preElementMock });
        preElementMock.QuerySelectorAllAsync("span").Returns(spanElementsMock);
    }
}