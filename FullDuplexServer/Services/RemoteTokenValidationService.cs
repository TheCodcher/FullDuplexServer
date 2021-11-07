using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FullDuplexServer.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using FullDuplexServer.Models;
using System.Security.Principal;
using Mapster;
using DuplexDomain.Dto.OAuth;

namespace FullDuplexServer.Services
{
    public class RemoteTokenValidationService : ITokenValidationService
    {
        public const string HTTP_CLIENT_NAME = "tokenServiceClient";

        IHttpClientFactory _httpClientFactory;
        IMemoryCache _cache;
        ILogger _logger;

        public RemoteTokenValidationService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ILoggerFactory loggerFactory)
        {
            _httpClientFactory = httpClientFactory;
            _cache = memoryCache;
            _logger = loggerFactory.CreateLogger<RemoteTokenValidationService>();
        }

        private HttpClient CreateHttpClient() => _httpClientFactory.CreateClient(HTTP_CLIENT_NAME);

        public async Task<IIdentity> ValidateAsync(string token)
        {
            if (_cache.TryGetValue(token, out UserIdentity user))
            {
                return user;
            }
            else
            {
                return await GetRemouteIdentity(token);
            }
        }

        private async Task<UserIdentity> GetRemouteIdentity(string token)
        {
            //return new UserIdentity { IsAuthenticated = true, AuthenticationType = "Bearer", Name = "Aboba" };
            var client = CreateHttpClient();

            var request = CreateTokenValidationRequest(token);
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"recieved code {response.StatusCode} from OAuth service ('{client.BaseAddress}'): '{responseContent}'");
                return null;
            }

            var answer = JsonSerializer.Deserialize<OAuthIdentityResponseDto>(responseContent);
            if (answer is null)
            {
                _logger.LogError($"recieved data from OAuth service ('{client.BaseAddress}') does not fit the model: '{responseContent}'");
                return null;
            }

            var answerIdentity = answer.Adapt<TokenServiceDto>();
            _cache.Set(token, answerIdentity.Identity, answerIdentity.DataLifeTime);

            return answerIdentity.Identity;
        }

        private HttpRequestMessage CreateTokenValidationRequest(string token)
        {
            var model = new OAuthIdentityRequestDto { Token = token };
            var serialized = JsonSerializer.Serialize(model);
            var content = new StringContent(serialized);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                Content = content
            };

            return request; 
        }
    }
}
