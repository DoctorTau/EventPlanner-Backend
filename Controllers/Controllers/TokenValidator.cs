using Microsoft.AspNetCore.Http;

namespace EventPlanner.Controllers
{
    public static class TokenValidator
    {
        public static string Token = Environment.GetEnvironmentVariable("TOKEN")
            ?? throw new InvalidOperationException("TOKEN environment variable is not set");

        public static bool CheckToken(IHeaderDictionary headers)
        {
            if (!headers.TryGetValue("bot-token", out var tokenHeader))
            {
                return false;
            }

            var expectedToken = Environment.GetEnvironmentVariable("TOKEN");
            return tokenHeader == expectedToken;
        }
    }
}