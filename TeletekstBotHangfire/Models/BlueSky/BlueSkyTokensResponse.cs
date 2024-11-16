// ReSharper disable InconsistentNaming
// ReSharper disable PropertyCanBeMadeInitOnly.Global
using System.Text.Json.Serialization;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace TeletekstBotHangfire.Models.BlueSky;

public class BlueSkyTokensResponse
{
    public required string did { get; set; }
    public required DidDoc didDoc { get; set; }
    public required string handle { get; set; }
    public required string email { get; set; }
    public required bool emailConfirmed { get; set; }
    public required bool emailAuthFactor { get; set; }
    public required string accessJwt { get; set; }
    public required string refreshJwt { get; set; }
    public required bool active { get; set; }
    
    public class DidDoc
    {
        [JsonPropertyName("@context")]
        public required string[] _context { get; set; }
        public required string id { get; set; }
        public required string[] alsoKnownAs { get; set; }
        public required VerificationMethod[] verificationMethod { get; set; }
        public required Service[] service { get; set; }
    }

    public class VerificationMethod
    {
        public required string id { get; set; }
        public required string type { get; set; }
        public required string controller { get; set; }
        public required string publicKeyMultibase { get; set; }
    }

    public class Service
    {
        public required string id { get; set; }
        public required string type { get; set; }
        public required string serviceEndpoint { get; set; }
    }
}



