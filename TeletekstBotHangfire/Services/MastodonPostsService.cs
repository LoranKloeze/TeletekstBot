using Mastonet;
using Mastonet.Entities;
using TeletekstBotHangfire.Models.Ef;
using TeletekstBotHangfire.Services.Interfaces;
using TeletekstBotHangfire.Utils;

namespace TeletekstBotHangfire.Services;


public class MastodonPostsService(IMastodonClient mastodonClient) : IMastodonPostsService
{
    public async Task SendTeletekstPageAsync(TeletekstPage page)
    {
        await using var screenshotStream = new MemoryStream(page.Screenshot);
        var attachment = await mastodonClient.UploadMedia(screenshotStream, "screenshot.jpg", 
            TeletekstPageUtils.AltText(page), 
            new AttachmentFocusData
        {
            X = 0.0,
            Y = 1.0
        });
        
        var body = TeletekstPageUtils.TitleForSocialMedia(page);
        await mastodonClient.PublishStatus(body, Visibility.Unlisted, 
            null, new List<string> { attachment.Id });
    }
}