using Serilog;
using TeletekstBotHangfire.Data;
using TeletekstBotHangfire.Models;
using TeletekstBotHangfire.Models.Ef;
using TeletekstBotHangfire.Services.Interfaces;
using TeletekstBotHangfire.Utils;

namespace TeletekstBotHangfire.Jobs;

public class PostNewPagesJob(ApplicationDbContext context, 
    ITeletekstPageService teletekstPageService, 
    ICurrentPagesService currentPagesService,
    IBlueSkyPostsService blueSkyPostsService,
    IMastodonPostsService mastodonPostsService
    )
{
    
    [Hangfire.AutomaticRetry(Attempts = 0)]
    public async Task StartAsync(PostNewPagesJobOptions options)
    {
        var headlinePageNumbers = await currentPagesService.GetPageNumbersAsync();
        foreach (var pageNumber in headlinePageNumbers)
        {
            var pageInDb = await context.TeletekstPages.FindAsync(pageNumber);
            var pageAtNos = await teletekstPageService.GetPageAsync(pageNumber);
            if (pageAtNos == null)
            {
                continue;
            }

            var changes = TeletekstPageUtils.Changes(pageInDb, pageAtNos);
            if (changes != PageChanges.NoChange)
            {
                await UpsertPageAsync(pageAtNos);
                pageAtNos.LastPageChanges = changes;
                if (options.PostToSocialMedia)
                {
                    Log.Information("[PostNewPagesJob] Posting updated page {PageNr} - {Title} - {Changes}", 
                        pageAtNos.PageNr, pageAtNos.Title, pageAtNos.LastPageChanges);
                    await blueSkyPostsService.SendTeletekstPageAsync(pageAtNos);
                    await mastodonPostsService.SendTeletekstPageAsync(pageAtNos);
                }
                else
                {
                    Log.Information("[PostNewPagesJob] Would have posted new page {PageNr} - {Title} - {Changes}", 
                        pageAtNos.PageNr, pageAtNos.Title, pageAtNos.LastPageChanges);
                }
                
                

            } 
            else
            {
                Log.Information("[PostNewPagesJob] No change {PageNr} - {Title}", pageAtNos.PageNr, pageAtNos.Title);
            }

        }
    }

    private async Task UpsertPageAsync(TeletekstPage pageAtNos)
    {
        var page = await context.TeletekstPages.FindAsync(pageAtNos.PageNr);
        if (page == null)
        {
            page = new TeletekstPage
            {
                PageNr = pageAtNos.PageNr,
                Title = pageAtNos.Title,
                Content = pageAtNos.Content,
                Screenshot = pageAtNos.Screenshot ,
                LastUpdatedInDbAt = DateTime.Now.ToUniversalTime()

            };
            context.TeletekstPages.Add(page);
        }
        else
        {
            page.Title = pageAtNos.Title;
            page.Content = pageAtNos.Content;
            page.LastUpdatedInDbAt = DateTime.Now.ToUniversalTime();
            page.Screenshot = pageAtNos.Screenshot;
            context.TeletekstPages.Update(page);
        }
        await context.SaveChangesAsync();
    }

}

public class PostNewPagesJobOptions
{
    public bool PostToSocialMedia { get; init; }
}