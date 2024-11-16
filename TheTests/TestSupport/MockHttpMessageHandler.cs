using System.Net;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace TheTests.TestSupport;
// Inspiration from https://dev.to/n_develop/mocking-the-httpclient-in-net-core-with-nsubstitute-k4j
public class MockHttpMessageHandler(IEnumerable<MockHttpMessageHandler.Handler> handlers) : HttpMessageHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var handler = handlers.FirstOrDefault(h => h.Url == request.RequestUri?.ToString());
        if (handler == null)
        {
            throw new Exception($"No handler found for {request.RequestUri}");
        }
        
        handler.NumberOfCalls++;
        if (request.Content != null) // Could be a GET-request without a body
        {
            handler.Input = await request.Content.ReadAsStringAsync(cancellationToken);
        }
        
        return new HttpResponseMessage(handler.StatusCode)
        {
            Content = new StringContent(handler.Response)
        };
    }

    public class Handler
    {
        public required string Url { get; set; }
        public required string Response { get; set; }
        public required HttpStatusCode StatusCode { get; set; }

        public string Input { get; set; } = string.Empty;
        public int NumberOfCalls { get; set; }
    }
}