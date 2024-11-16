using Microsoft.Playwright;
using TeletekstBotHangfire.Services.Interfaces;
using TeletekstBotHangfire.Services.Utils;

namespace TeletekstBotHangfire.Services;

public class CurrentPagesService(HeadlinesScraper scraper) : ICurrentPagesService
{
    public async Task<IEnumerable<int>> GetPageNumbersAsync()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var context = await browser.NewContextAsync();
        var browserPage = await context.NewPageAsync();
        var headlines = await scraper.RetrieveHeadlinesAsync(browserPage);
        return headlines.Select(headline => headline.PageNr).ToList();
    }
}