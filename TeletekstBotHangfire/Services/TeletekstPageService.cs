﻿using Microsoft.Playwright;
using TeletekstBotHangfire.Models;
using TeletekstBotHangfire.Services.Interfaces;
using TeletekstBotHangfire.Services.Utils;

namespace TeletekstBotHangfire.Services;

public class TeletekstPageService(TeletekstPageScraper scraper) : ITeletekstPageService
{
    public async Task<TeletekstPage> GetPageAsync(int teletekstPageNr)
    {
        
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var context = await browser.NewContextAsync();
        var browserPage = await context.NewPageAsync();
        return await scraper.RetrievePageAsync(browserPage, teletekstPageNr);
    }
}