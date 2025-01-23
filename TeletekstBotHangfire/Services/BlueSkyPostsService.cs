using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TeletekstBotHangfire.Models.BlueSky;
using TeletekstBotHangfire.Models.Ef;
using TeletekstBotHangfire.Services.Interfaces;
using TeletekstBotHangfire.Utils;

namespace TeletekstBotHangfire.Services;

public class BlueSkyPostsService : IBlueSkyPostsService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<BlueSkyPostsService> _logger;
    
    private readonly string _blueSkyIdentifier;
    private readonly string _blueSkyPassword;

    private const string AccessTokenCacheKey = "BlueSkyAccessToken";

    private const string ApiUrl = "https://bsky.social";

    public BlueSkyPostsService(HttpClient httpClient, IConfiguration configuration, IMemoryCache memoryCache, 
        ILogger<BlueSkyPostsService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _memoryCache = memoryCache;
        _logger = logger;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        _httpClient.BaseAddress = new Uri(ApiUrl);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        CheckConfiguration();
        
        _blueSkyIdentifier = _configuration["BlueSky:Identifier"]!;
        _blueSkyPassword = _configuration["BlueSky:Password"]!;
        
    }
    public async Task SendTeletekstPageAsync(TeletekstPage page)
    {
        var blobResponse = await PostBlobAsync(page.Screenshot);
        await PostRecordAsync(page, blobResponse.blob);
    }

    private async Task<BlueSkyBlobResponse> PostBlobAsync(byte[] blob)
    {
        await RefreshTokensAsync();
        var blobContent = new ByteArrayContent(blob);
        blobContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        var response = await _httpClient.PostAsync("/xrpc/com.atproto.repo.uploadBlob", 
            blobContent);
        var responseContent = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();        
        
        var responseObject = JsonSerializer.Deserialize<BlueSkyBlobResponse>(responseContent);
        if (responseObject == null)
        {
            throw new Exception("Failed to deserialize BlueSkyBlobResponse");
        }

        return responseObject;
    }

    private async Task PostRecordAsync(TeletekstPage page, BlueSkyBlobBody blobBody)
    {
        await RefreshTokensAsync();
        var requestBody = BuildPost(page, blobBody);
        var response = await _httpClient.PostAsync("/xrpc/com.atproto.repo.createRecord", 
            new StringContent(Serialize(requestBody), Encoding.UTF8, 
                "application/json"));
        await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
    }
    
    
    private async Task RefreshTokensAsync()
    {
        if (_memoryCache.TryGetValue(AccessTokenCacheKey, out string? accessToken))
        {
            _logger.LogInformation("BlueSky: using cached access token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return;
        }
        _logger.LogInformation("BlueSky: access tokens not found in cache, getting a new one");
        
        var requestBody = new BlueSkyTokensRequest
        {
            Identifier = _blueSkyIdentifier,
            Password = _blueSkyPassword
        };
        var response = await _httpClient.PostAsync("/xrpc/com.atproto.server.createSession", 
            new StringContent(Serialize(requestBody), Encoding.UTF8, 
                "application/json"));
        
        var responseContent = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        
        var responseObject = JsonSerializer.Deserialize<BlueSkyTokensResponse>(responseContent);
        if (responseObject == null)
        {
            throw new Exception("Failed to deserialize BlueSkyTokensResponse");
        }
        // Set the access token in cache
        _memoryCache.Set(AccessTokenCacheKey, responseObject.accessJwt, TimeSpan.FromMinutes(90));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", responseObject.accessJwt);
    } 
    
    private void CheckConfiguration()
    {
        if (string.IsNullOrEmpty(_configuration["BlueSky:Identifier"]) 
            || string.IsNullOrEmpty(_configuration["BlueSky:Password"]))
        {
            throw new Exception("BlueSky configuration is missing or incomplete");
        }
    }

    private BlueSkyPostRequest BuildPost(TeletekstPage page, BlueSkyBlobBody blobBody)
    {
        var title = TeletekstPageUtils.TitleForSocialMedia(page);
        var change = TeletekstPageUtils.ChangesText(page);
        var text = change == null ? title : $"{title} {change}";
        
        return new BlueSkyPostRequest
        {
            Repo = _blueSkyIdentifier,
            Collection = "app.bsky.feed.post",
            Record = new Record
            {
                Text = text,
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Facets = [
                    new Record.Facet
                    {
                        Index = new Record.Facet.FacetIndex
                        {
                            ByteStart = 6,
                            ByteEnd = Encoding.UTF8.GetByteCount(title)
                        },
                        Features = [
                        new Record.Facet.Feature
                        {
                            Type = "app.bsky.richtext.facet#link",
                            Uri = TeletekstPageUtils.LinkToNos(page)
                        }
                        ]
                    }
                ],
                Embed = new Record.RecordEmbed
                {
                    Type = "app.bsky.embed.images",
                    Images = [
                        new Record.RecordEmbed.Image
                        {
                            Alt = TeletekstPageUtils.AltText(page),
                            TheImage = blobBody
                        }
                    ]
                }
            }
        };
    }
    
    private string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj, _serializerOptions);
    }
}