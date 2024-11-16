using Serilog;
using TeletekstBotHangfire.Data;
using TeletekstBotHangfire.Models.Ef;
using TeletekstBotHangfire.Services;
using TeletekstBotHangfire.Services.Interfaces;

namespace TeletekstBotHangfire.Jobs;

public class PostNewPagesJob(ApplicationDbContext context, 
    ITeletekstPageService teletekstPageService, 
    ICurrentPagesService currentPagesService,
    IBlueSkyPostsService blueSkyPostsService
    )
{
    public async Task StartAsync()
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

            if (pageInDb == null || PageChanged(pageInDb, pageAtNos))
            {
                Log.Information("[PostNewPagesJob] Posting new page {PageNr} - {Title}", pageAtNos.PageNr, pageAtNos.Title);
                await blueSkyPostsService.SendTeletekstPageAsync(pageAtNos);
                await UpsertPageAsync(pageAtNos);
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
    
    private static bool PageChanged(TeletekstPage pageInDb, TeletekstPage pageAtNos)
    {
        if (pageAtNos.Title != pageInDb.Title)
        {
            return true;
        }
        
        return pageAtNos.Content != pageInDb.Content;
    }
}