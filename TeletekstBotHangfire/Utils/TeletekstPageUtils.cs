using System.Diagnostics.CodeAnalysis;
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
    
    public static PageChanges Changes(TeletekstPage? pageInDb, TeletekstPage pageAtNos)
    {
        if (pageInDb == null)
        {
            return PageChanges.NewPage;
        }
        
        if (pageAtNos.Title != pageInDb.Title && pageAtNos.Content != pageInDb.Content)
        {
            return PageChanges.ContentAndTitleChanged;
        }
        
        if (pageAtNos.Title != pageInDb.Title)
        {
            return PageChanges.TitleChanged;
        }
        
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (pageAtNos.Content != pageInDb.Content)
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

    public static bool ShouldPostPage(TeletekstPage? thisPageInDb, [NotNullWhen(true)] TeletekstPage? pageAtNos,
        List<TeletekstPage> pagesInDb)
    {
        if (pageAtNos == null || string.IsNullOrWhiteSpace(pageAtNos.Title))
        {
            return false;
        }

        var titleAlreadyInDbForOtherPage = pagesInDb
            .Any(pageInDb => pageInDb.PageNr != pageAtNos.PageNr && pageInDb.Title == pageAtNos.Title);
        if (titleAlreadyInDbForOtherPage)
        {
            return false;
        }


        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (Changes(thisPageInDb, pageAtNos) != PageChanges.NoChange)
        {
            return true;
        }

        return false;
    }
}