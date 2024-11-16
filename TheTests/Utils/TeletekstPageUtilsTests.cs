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
}