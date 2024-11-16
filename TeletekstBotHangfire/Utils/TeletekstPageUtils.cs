using TeletekstBotHangfire.Models.Ef;

namespace TeletekstBotHangfire.Utils;

public static class TeletekstPageUtils
{
    public static string TitleForSocialMedia(TeletekstPage page)
    {
        return $"[{page.PageNr}] {page.Title}";
    }

    public static string LinkToNos(TeletekstPage page)
    {
        return $"https://nos.nl/teletekst/{page.PageNr}";
    }

    public static string AltText(TeletekstPage page)
    {
        return $"Pagina: {page.PageNr} - Titel: {page.Title} - Inhoud: {page.Content}";
    }
}