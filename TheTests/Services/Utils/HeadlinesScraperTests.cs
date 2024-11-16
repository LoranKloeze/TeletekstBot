using Microsoft.Playwright;
using NSubstitute;
using TeletekstBotHangfire.Services.Utils;

namespace TheTests.Services.Utils;

[TestFixture]
public class HeadlinesScraperTests
{
    private HeadlinesScraper _scraper;
    private IPage _mockPage;
#pragma warning disable NUnit1032
    private IElementHandle _mockElementHandle;
#pragma warning restore NUnit1032

    [SetUp]
    public void Setup()
    {
        _scraper = new HeadlinesScraper();
        _mockPage = Substitute.For<IPage>();
        _mockElementHandle = Substitute.For<IElementHandle>();
    }

    [Test]
    public async Task RetrieveHeadlinesAsync_ShouldReturnHeadlines_WhenValidSpansArePresent()
    {
        // Arrange
        var spanElements = new List<IElementHandle> { _mockElementHandle, _mockElementHandle };
        _mockPage.QuerySelectorAllAsync("span").Returns(spanElements);
        
        _mockElementHandle.InnerTextAsync().Returns("Headline", "123");
        _mockElementHandle.GetAttributeAsync("class").Returns("cyan", "yellow");

        // Act
        var headlines = (await _scraper.RetrieveHeadlinesAsync(_mockPage)).ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(headlines, Has.Exactly(1).Items);
            Assert.That(headlines.First().Title, Is.EqualTo("Headline"));
            Assert.That(headlines.First().PageNr, Is.EqualTo(123));
        });
    }
    
    [Test]
    public async Task RetrieveHeadlinesAsync_RemovesTrailingDotsFromTitles()
    {
        // Arrange
        var spanElements = new List<IElementHandle> { _mockElementHandle, _mockElementHandle };
        _mockPage.QuerySelectorAllAsync("span").Returns(spanElements);
        
        _mockElementHandle.InnerTextAsync().Returns("Headline...", "123");
        _mockElementHandle.GetAttributeAsync("class").Returns("cyan", "yellow");

        // Act
        var headlines = (await _scraper.RetrieveHeadlinesAsync(_mockPage)).ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(headlines, Has.Exactly(1).Items);
            Assert.That(headlines.First().Title, Is.EqualTo("Headline"));
        });
    }
    
    [Test]
    public async Task RetrieveHeadlinesAsync_RemovesLeadingSpacesFromTitles()
    {
        // Arrange
        var spanElements = new List<IElementHandle> { _mockElementHandle, _mockElementHandle };
        _mockPage.QuerySelectorAllAsync("span").Returns(spanElements);
        
        _mockElementHandle.InnerTextAsync().Returns("  Headline", "123");
        _mockElementHandle.GetAttributeAsync("class").Returns("cyan", "yellow");

        // Act
        var headlines = (await _scraper.RetrieveHeadlinesAsync(_mockPage)).ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(headlines, Has.Exactly(1).Items);
            Assert.That(headlines.First().Title, Is.EqualTo("Headline"));
        });
    }

    [Test]
    public async Task RetrieveHeadlinesAsync_ShouldReturnEmpty_WhenNoValidSpans()
    {
        // Arrange
        _mockPage.QuerySelectorAllAsync("span").Returns(new List<IElementHandle>());

        // Act
        var headlines = await _scraper.RetrieveHeadlinesAsync(_mockPage);

        // Assert
        Assert.That(headlines, Is.Empty);
    }

}