using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TeletekstBotHangfire.Models.BlueSky;
using TeletekstBotHangfire.Models.Ef;
using TeletekstBotHangfire.Services;
using TheTests.TestSupport;

namespace TheTests.Services;

[TestFixture]
public class BlueSkyPostsServiceTests
{
    private const string BlueSkyIdentifier = "test_identifier";
    private const string BlueSkyPassword = "test_password";
    
    private IConfiguration _configuration;

    [SetUp]
    public void Setup()
    {
        _configuration = Substitute.For<IConfiguration>();
        _configuration["BlueSky:Identifier"].Returns(BlueSkyIdentifier);
        _configuration["BlueSky:Password"].Returns(BlueSkyPassword);
    }

    [Test]
    public async Task SendTeletekstPageAsync_Should_PostBlob_And_PostRecord()
    {
        // Arrange
        var page = new TeletekstPage
        {
            PageNr = 101,
            Screenshot =  "13337"u8.ToArray(),
            Title = "Nieuwsitem",
            Content = "Inhoud van het nieuwsitem"
        };

        // Mock response for token refresh
        var tokenResponse = new BlueSkyTokensResponse
        {
            did = "",
            didDoc = new BlueSkyTokensResponse.DidDoc
            {
                _context = [],
                id = "",
                alsoKnownAs = [],
                verificationMethod = [],
                service = []
            },
            handle = "",
            email = "",
            emailConfirmed = false,
            emailAuthFactor = false,
            accessJwt = "",
            refreshJwt = "",
            active = false
        };
        var tokenHandler = new MockHttpMessageHandler.Handler
        {
            Url = "https://bsky.social/xrpc/com.atproto.server.createSession",
            Response = JsonSerializer.Serialize(tokenResponse),
            StatusCode = HttpStatusCode.OK
        };

        // Mock response for blob upload
        var blobResponse = new BlueSkyBlobResponse
        {
            blob = new BlueSkyBlobBody
            {
                _type = "",
                _ref = new BlueSkyBlobBody.Ref
                {
                    link = ""
                },
                mimeType = "",
                size = 0
            }
        };
        var blobHandler = new MockHttpMessageHandler.Handler
        {
            Url = "https://bsky.social/xrpc/com.atproto.repo.uploadBlob",
            Response = JsonSerializer.Serialize(blobResponse),
            StatusCode = HttpStatusCode.OK
        };
        
        // Mock response for record creation
        var recordHandler = new MockHttpMessageHandler.Handler
        {
            Url = "https://bsky.social/xrpc/com.atproto.repo.createRecord",
            Response = string.Empty,
            StatusCode = HttpStatusCode.OK
        };

        // Create HttpClient with the handlers
        List<MockHttpMessageHandler.Handler> handlers = [blobHandler, recordHandler, tokenHandler];
        var handler = new MockHttpMessageHandler(handlers);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://bsky.social")
        };
        
        // Create an IMemoryCache mock
        var memoryCache = Substitute.For<IMemoryCache>();
        
        // Create an ILogger mock
        var logger = Substitute.For<ILogger<BlueSkyPostsService>>();

        var service = new BlueSkyPostsService(httpClient, _configuration, memoryCache, logger);

        // Act
        await service.SendTeletekstPageAsync(page);

        Assert.Multiple(() =>
        {
            // Assert: Check that the token refresh call was made
            Assert.That(tokenHandler.NumberOfCalls, Is.EqualTo(2));
            Assert.That(tokenHandler.Input, Does.Contain(BlueSkyIdentifier));
            Assert.That(tokenHandler.Input, Does.Contain(BlueSkyPassword));

            // Assert: Check that the blob upload call was made
            Assert.That(blobHandler.NumberOfCalls, Is.EqualTo(1));
            Assert.That(blobHandler.Input, Does.Contain("13337"));

            // Assert: Check that the record creation call was made
            Assert.That(recordHandler.NumberOfCalls, Is.EqualTo(1));
            Assert.That(recordHandler.Input, Does.Contain("app.bsky.feed.post"));
            Assert.That(recordHandler.Input, Does.Contain("Nieuwsitem"));
            Assert.That(recordHandler.Input, Does.Contain("Inhoud"));
        });
    }
}
