using System.Text.Json.Serialization;
// ReSharper disable InconsistentNaming
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace TeletekstBotHangfire.Models.BlueSky;

public class BlueSkyPostRequest
{
    public required string Repo { get; set; }

    public required string Collection { get; set; }

    public required Record Record { get; set; }
}

public class Record
{
    public required string Text { get; set; }

    public required string CreatedAt { get; set; }

    public required List<Facet> Facets { get; set; }

    public required RecordEmbed Embed { get; set; }
    
    public class Facet
    {
        public required FacetIndex Index { get; set; }

        public required List<Feature> Features { get; set; }
        
        public class Feature
        {
            [JsonPropertyName("$type")]
            public required string Type { get; set; }

            public required string Uri { get; set; }
        }

        public class FacetIndex
        {
            public required int ByteStart { get; set; }

            public required int ByteEnd { get; set; }
        }
    }
    
    public class RecordEmbed
    {
        [JsonPropertyName("$type")]
        public required string Type { get; set; }

        public required List<Image> Images { get; set; }
    
        public class Image
        {
            public required string Alt { get; set; }
            
            
            [JsonPropertyName("image")]
            public required BlueSkyBlobBody TheImage { get; set; }
        }
    }
}








