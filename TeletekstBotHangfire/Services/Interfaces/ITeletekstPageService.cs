using TeletekstBotHangfire.Models.Ef;

namespace TeletekstBotHangfire.Services.Interfaces;

public interface ITeletekstPageService
{
    Task<TeletekstPage?> GetPageAsync(int teletekstPageNr);
}