using System.Text.Json.Serialization;
// ReSharper disable InconsistentNaming
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace TeletekstBotHangfire.Models.BlueSky;

// ReSharper disable once ClassNeverInstantiated.Global
public class BlueSkyBlobBody
{
    [JsonPropertyName("$type")]
    public required string _type { get; set; }
    [JsonPropertyName("ref")]
    public required Ref _ref { get; set; }
    public required string mimeType { get; set; }
    public required int size { get; set; }
        
    public class Ref
    {
        [JsonPropertyName("$link")]
        public required string link { get; set; }
    }
}