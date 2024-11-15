using Hangfire.Dashboard;

namespace TeletekstBotHangfire.Utils.Hangfire;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var config = context.GetHttpContext().RequestServices.GetRequiredService<IConfiguration>();
        var expectedKey = $"Key {config["Hangfire:LoginKey"]}";
        if (string.IsNullOrEmpty(expectedKey))
        {
            throw new InvalidOperationException("Hangfire login key not found in configuration.");
        }
        
        var providedKey = context.GetHttpContext().Request.Headers.Authorization;
        return providedKey == expectedKey;
    }
}