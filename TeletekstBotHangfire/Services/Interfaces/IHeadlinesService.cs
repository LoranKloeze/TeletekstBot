using TeletekstBotHangfire.Models;

namespace TeletekstBotHangfire.Services.Interfaces;

public interface IHeadlinesService
{
    Task<IEnumerable<Headline>> GetAllAsync();
}