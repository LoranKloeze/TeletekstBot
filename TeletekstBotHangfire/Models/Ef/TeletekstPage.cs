using System.ComponentModel.DataAnnotations;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace TeletekstBotHangfire.Models.Ef;

public class TeletekstPage
{
    [Key]
    public int PageNr { get; set; }
    
    [StringLength(64)]
    public required string Title { get; set; }
    
    [StringLength(1024)]
    public required string Content { get; set; }
    
    public required byte[] Screenshot { get; set; }
    
    public DateTime? LastUpdatedInDbAt { get; set; }
    
    public PageChanges? LastPageChanges { get; set; }
    
}