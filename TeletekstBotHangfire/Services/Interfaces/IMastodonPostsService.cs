using TeletekstBotHangfire.Models.Ef;

namespace TeletekstBotHangfire.Services.Interfaces;

public interface IMastodonPostsService
{
    Task SendTeletekstPageAsync(TeletekstPage page);
}