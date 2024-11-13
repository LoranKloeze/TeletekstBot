using TeletekstBotHangfire.Models;

namespace TeletekstBotHangfire.Services.Interfaces;

public interface ITeletekstPageService
{
    Task<TeletekstPage> GetPageAsync(int teletekstPageNr);
}