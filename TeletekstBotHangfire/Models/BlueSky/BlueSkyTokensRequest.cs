// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace TeletekstBotHangfire.Models.BlueSky;
// ReSharper disable InconsistentNaming
// ReSharper disable PropertyCanBeMadeInitOnly.Global

public class BlueSkyTokensRequest
{
    public required string Identifier { get; set; }
    public required string Password { get; set; }
}