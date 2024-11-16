namespace TeletekstBotHangfire.Services.Interfaces;

public interface ICurrentPagesService
{
    Task<IEnumerable<int>> GetPageNumbersAsync();
}