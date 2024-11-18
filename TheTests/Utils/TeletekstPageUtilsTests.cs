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
    
     [Test]
        public void ShouldPostPage_PageAtNosIsNull_ReturnsFalse()
        {
            // Arrange
            var pageInDb = new TeletekstPage
            {
                PageNr = 101,
                Title = "Title in DB",
                Content = "Content in DB",
                Screenshot = [],
                LastUpdatedInDbAt = DateTime.UtcNow
            };

            // Act
            var result = TeletekstPageUtils.ShouldPostPage(pageInDb, null);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldPostPage_PageAtNosHasEmptyTitle_ReturnsFalse()
        {
            // Arrange
            var pageAtNos = new TeletekstPage
            {
                PageNr = 101,
                Title = "   ",
                Content = "Some content",
                Screenshot = [],
                LastUpdatedInDbAt = DateTime.UtcNow
            };

            // Act
            var result = TeletekstPageUtils.ShouldPostPage(null, pageAtNos);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldPostPage_ChangesDetected_ReturnsTrue()
        {
            // Arrange
            var pageInDb = new TeletekstPage
            {
                PageNr = 101,
                Title = "Old Title",
                Content = "Old Content",
                Screenshot = [],
                LastUpdatedInDbAt = DateTime.UtcNow
            };

            var pageAtNos = new TeletekstPage
            {
                PageNr = 101,
                Title = "New Title",
                Content = "New Content",
                Screenshot = [],
                LastUpdatedInDbAt = DateTime.UtcNow
            };

            // Act
            var result = TeletekstPageUtils.ShouldPostPage(pageInDb, pageAtNos);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldPostPage_NoChangesDetected_ReturnsFalse()
        {
            // Arrange
            var pageInDb = new TeletekstPage
            {
                PageNr = 101,
                Title = "Same Title",
                Content = "Same Content",
                Screenshot = [],
                LastUpdatedInDbAt = DateTime.UtcNow
            };

            var pageAtNos = new TeletekstPage
            {
                PageNr = 101,
                Title = "Same Title",
                Content = "Same Content",
                Screenshot = [],
                LastUpdatedInDbAt = DateTime.UtcNow
            };

            // Act
            var result = TeletekstPageUtils.ShouldPostPage(pageInDb, pageAtNos);

            // Assert
            Assert.That(result, Is.False);
        }
}