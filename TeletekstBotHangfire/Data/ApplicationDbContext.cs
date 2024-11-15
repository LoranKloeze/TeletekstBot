using Microsoft.EntityFrameworkCore;
using TeletekstBotHangfire.Models.Ef;

namespace TeletekstBotHangfire.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    
    public DbSet<TeletekstPage> TeletekstPages { get; set; }
    
}