using TeletekstBotHangfire.Models.Ef;

namespace TeletekstBotHangfire.Services.Interfaces;

public interface IBlueSkyPostsService
{
    Task SendTeletekstPageAsync(TeletekstPage page);
}