using System.Text.RegularExpressions;
using Microsoft.Playwright;
using TeletekstBotHangfire.Models.Ef;

namespace TeletekstBotHangfire.Services.Utils;

public class TeletekstPageScraper
{
    private const string PageUrl = "https://nos.nl/teletekst/";
    

    // ReSharper disable once MemberCanBeMadeStatic.Global
    public async Task<TeletekstPage?> RetrievePageAsync(IPage browserPage, int pageNr)
    {
        await browserPage.GotoAsync(BuildPageUrl(pageNr));

        var teletekstPage = new TeletekstPage
        {
            PageNr = pageNr,
            Title = await TitleTextAsync(browserPage),
            Content = await ContentTextAsync(browserPage),
            Screenshot = await ScreenshotAsync(browserPage)
        };

        return teletekstPage;

    }

    private static async Task<string> TitleTextAsync(IPage browserPage)
    {
        var blocks = await TextBlocksAsync(browserPage);
        var dirtyTitle = blocks[4];

        return CleanUpText(dirtyTitle);
    }

    private static async Task<List<string>> TextBlocksAsync(IPage browserPage)
    {
        var pres = await browserPage.QuerySelectorAllAsync("pre");
        var pre = pres[0];

        if (pre == null)
        {
            throw new Exception("No <pre> found in the page, it should be there");
        }
        
        var spansInPre = await pre.QuerySelectorAllAsync("span");
        var blocks = await Task.WhenAll(spansInPre.Select(async span => await span.InnerTextAsync()));
        return blocks.ToList();
    }


    private static async Task<string> ContentTextAsync(IPage browserPage)
    {
        const int firstBlocksToSkip = 6;
        const int lastBlocksToSkip = 4;

        var blocks = await TextBlocksAsync(browserPage);
        var contentBlocks = blocks.Skip(firstBlocksToSkip)
            .Take(blocks.Count - firstBlocksToSkip - lastBlocksToSkip).ToList();
        
        var dirtyContent = string.Join("", contentBlocks);
        return CleanUpText(dirtyContent);
    }

    private static async Task<byte[]> ScreenshotAsync(IPage browserPage)
    {
        return await browserPage.ScreenshotAsync(new PageScreenshotOptions
        {
            Clip = new Clip
            {
                X = 445,
                Y = 118,
                Width = 507,
                Height = 551
            },
            Type = ScreenshotType.Png
        });
    }

    private static string CleanUpText(string dirtyContent)
    {
        var cleanContent = dirtyContent.Trim();

#pragma warning disable SYSLIB1045
        cleanContent = Regex.Replace(cleanContent, @"\s+", " ");
#pragma warning restore SYSLIB1045
        
#pragma warning disable SYSLIB1045
        cleanContent = Regex.Replace(cleanContent, @"[^\p{L}\p{N}\s\-',._]", "");
#pragma warning restore SYSLIB1045

        return cleanContent;
    }


    private static string BuildPageUrl(int pageNr)
    {
        return $"{PageUrl}{pageNr}";
    }
}