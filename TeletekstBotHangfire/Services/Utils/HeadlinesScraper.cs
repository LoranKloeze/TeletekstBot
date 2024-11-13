using Microsoft.Playwright;
using TeletekstBotHangfire.Models;

namespace TeletekstBotHangfire.Services.Utils;

public class HeadlinesScraper
{
    private const string HeadlinesUrl = "https://nos.nl/teletekst/101";

    // ReSharper disable once MemberCanBeMadeStatic.Global
    public async Task<IEnumerable<Headline>> RetrieveHeadlinesAsync(IPage browserPage)
    {
        await browserPage.GotoAsync(HeadlinesUrl);
        var spans = await browserPage.QuerySelectorAllAsync("span");

        List<Headline> headlines = [];
        for (var i = 0; i < spans.Count; i++)
        {
            if (i + 1 >= spans.Count) break;

            var classesThisSpan = await ExtractClassesAsync(spans[i]);
            var classesNextSpan = await ExtractClassesAsync(spans[i + 1]);
            var textThisSpan = await spans[i].InnerTextAsync();
            var textNextSpan = await spans[i + 1].InnerTextAsync();

            var nextSpanHasNumber = int.TryParse(textNextSpan, out var nrNextSpan);

            if (nextSpanHasNumber &&
                classesThisSpan.Contains("cyan") &&
                classesNextSpan.Contains("yellow"))
            {
                headlines.Add(new Headline(CleanTitle(textThisSpan), nrNextSpan));
            }
        }

        return headlines;
    }

    private static async Task<IEnumerable<string>> ExtractClassesAsync(IElementHandle element)
    {
        var classAttr = await element.GetAttributeAsync("class");
        return classAttr == null ? [] : classAttr.Split(" ");
    }

    private static string CleanTitle(string dirtyTitle)
    {
        var cleanTitle = dirtyTitle.TrimEnd('.', ' ');
        cleanTitle = cleanTitle.TrimStart(' ');
        return cleanTitle;
    }
}