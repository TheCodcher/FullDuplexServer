using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FullDuplexServer.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace FullDuplexServer
{
    public class AuthHandler : AuthenticationHandler<AuthHandlerOpt>
    {
        public const string SCHEME = "Bearer";

        public AuthHandler(
            IOptionsMonitor<AuthHandlerOpt> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock,
            ITokenValidationService validationService) : base(options, logger, encoder, clock)
        {
            _validationService = validationService;
            _logger = logger.CreateLogger<AuthHandler>();
        }

        ITokenValidationService _validationService;
        ILogger _logger;

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return Unauthorized();

            string authorizationHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return AuthenticateResult.NoResult();
            }

            if (!authorizationHeader.StartsWith(SCHEME, StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized();
            }

            string token = authorizationHeader.Substring(SCHEME.Length).Trim();

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            try
            {
                var identity = await _validationService.ValidateAsync(token);
                if (identity is null || !identity.IsAuthenticated)
                {
                    Unauthorized();
                }

                var principal = new ClaimsPrincipal(identity);

                var ticket = new AuthenticationTicket(principal, SCHEME);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError($"unhandled exception: {ex}");
                return Unauthorized();
            }
        }

        private AuthenticateResult Unauthorized() => AuthenticateResult.Fail("Unauthorized");
    }

    public class AuthHandlerOpt : AuthenticationSchemeOptions { }
}
