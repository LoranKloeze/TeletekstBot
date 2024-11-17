using TeletekstBotHangfire.Models;
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
    
    public static PageChanges Changes(TeletekstPage? pageA, TeletekstPage pageB)
    {
        if (pageA == null)
        {
            return PageChanges.NewPage;
        }
        
        if (pageB.Title != pageA.Title && pageB.Content != pageA.Content)
        {
            return PageChanges.ContentAndTitleChanged;
        }
        
        if (pageB.Title != pageA.Title)
        {
            return PageChanges.TitleChanged;
        }
        
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (pageB.Content != pageA.Content)
        {
            return PageChanges.ContentChanged;
        }
        
        return PageChanges.NoChange;
    }

    public static string? ChangesText(TeletekstPage page)
    {
        // ReSharper disable once ConvertSwitchStatementToSwitchExpression
        switch (page.LastPageChanges)
        {
            case PageChanges.ContentChanged:
                return "(Update: inhoud)";
            case PageChanges.NoChange:
            case PageChanges.NewPage:
            case PageChanges.TitleChanged:
            case PageChanges.ContentAndTitleChanged:
            case null:
                return null;
            default:
                throw new ArgumentOutOfRangeException(nameof(page));
        }
    }
}