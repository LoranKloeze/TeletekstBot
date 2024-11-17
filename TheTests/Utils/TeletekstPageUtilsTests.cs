using TeletekstBotHangfire.Models;
using TeletekstBotHangfire.Models.Ef;
using TeletekstBotHangfire.Utils;

namespace TheTests.Utils;

[TestFixture]
public class TeletekstPageUtilsTests
{
    private TeletekstPage _samplePage;

    [SetUp]
    public void SetUp()
    {
        _samplePage = new TeletekstPage
        {
            PageNr = 101,
            Title = "Nieuws",
            Content = "Dit is een test inhoud voor Teletekst pagina.",
            Screenshot = []
        };
    }

    [Test]
    public void TitleForSocialMedia_ReturnsCorrectFormat()
    {
        // Arrange
        const string expected = "[101] Nieuws";

        // Act
        var result = TeletekstPageUtils.TitleForSocialMedia(_samplePage);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void LinkToNos_ReturnsCorrectUrl()
    {
        // Arrange
        const string expected = "https://nos.nl/teletekst/101";

        // Act
        var result = TeletekstPageUtils.LinkToNos(_samplePage);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void AltText_ReturnsCorrectAltText()
    {
        // Arrange
        const string expected = "Pagina: 101 - Titel: Nieuws - Inhoud: Dit is een test inhoud voor Teletekst pagina.";

        // Act
        var result = TeletekstPageUtils.AltText(_samplePage);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
    
        [Test]
    public void Changes_ShouldReturn_NewPage_WhenPageAIsNull()
    {
        // Arrange
        TeletekstPage? pageA = null;
        var pageB = new TeletekstPage
        {
            Title = "Title",
            Content = "Content",
            Screenshot = []
        };

        // Act
        var result = TeletekstPageUtils.Changes(pageA, pageB);

        // Assert
        Assert.That(result, Is.EqualTo(PageChanges.NewPage));
    }

    [Test]
    public void Changes_ShouldReturn_NoChange_WhenPagesAreIdentical()
    {
        // Arrange
        var pageA = new TeletekstPage { Title = "Title", Content = "Content", Screenshot = []};
        var pageB = new TeletekstPage { Title = "Title", Content = "Content", Screenshot = []};

        // Act
        var result = TeletekstPageUtils.Changes(pageA, pageB);

        // Assert
        Assert.That(result, Is.EqualTo(PageChanges.NoChange));
    }

    [Test]
    public void Changes_ShouldReturn_ContentAndTitleChanged_WhenBothTitleAndContentDiffer()
    {
        // Arrange
        var pageA = new TeletekstPage { Title = "Title A", Content = "Content A", Screenshot = []};
        var pageB = new TeletekstPage { Title = "Title B", Content = "Content B", Screenshot = []};

        // Act
        var result = TeletekstPageUtils.Changes(pageA, pageB);

        // Assert
        Assert.That(result, Is.EqualTo(PageChanges.ContentAndTitleChanged));
    }

    [Test]
    public void Changes_ShouldReturn_TitleChanged_WhenOnlyTitleDiffers()
    {
        // Arrange
        var pageA = new TeletekstPage { Title = "Title A", Content = "Content", Screenshot = []};
        var pageB = new TeletekstPage { Title = "Title B", Content = "Content", Screenshot = []};

        // Act
        var result = TeletekstPageUtils.Changes(pageA, pageB);

        // Assert
        Assert.That(result, Is.EqualTo(PageChanges.TitleChanged));
    }

    [Test]
    public void Changes_ShouldReturn_ContentChanged_WhenOnlyContentDiffers()
    {
        // Arrange
        var pageA = new TeletekstPage { Title = "Title", Content = "Content A", Screenshot = []};
        var pageB = new TeletekstPage { Title = "Title", Content = "Content B", Screenshot = []};

        // Act
        var result = TeletekstPageUtils.Changes(pageA, pageB);

        // Assert
        Assert.That(result, Is.EqualTo(PageChanges.ContentChanged));
    }
}