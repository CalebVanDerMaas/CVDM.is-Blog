using System.Security.Claims;
using System.Text.Encodings.Web;
using CVDMBlog.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CVDMBlog.Auth
{
    public class ApiKeyAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string ApiKeyHeaderName = "X-API-Key";
        private readonly IAwsSecretsService _awsSecretsService;
        
        public ApiKeyAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IAwsSecretsService awsSecretsService) // Inject IAwsSecretsService
            : base(options, logger, encoder, clock)
        {
            _awsSecretsService = awsSecretsService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Fetch the API Key from Environment Variables
            string apiKeyFromEnv = Environment.GetEnvironmentVariable("CVDM_Blog_Status_Api_Key");

            if (string.IsNullOrEmpty(apiKeyFromEnv))
            {
                return AuthenticateResult.Fail("API Key was not found in environment variables.");
            }

            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                return AuthenticateResult.Fail("API Key was not provided.");
            }

            var providedApiKey = apiKeyHeaderValues.ToString();

            if (providedApiKey == apiKeyFromEnv)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "API") };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.Fail("Invalid API Key.");
        }


    }
}