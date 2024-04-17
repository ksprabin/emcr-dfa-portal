using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EMBC.Utilities.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using YamlDotNet.Core.Tokens;

namespace EMBC.DFA.API.Services
{
    public class ADFSTokenProvider : ITokenProvider
    {
        private ADFSTokenProviderOptions options;
        private readonly IHttpClientFactory httpClientFactory;
        private const int Buffer = 60;
        private const string tokenkey = "oauth-token";
        private readonly IMemoryCache mcache;

        public ADFSTokenProvider(IHttpClientFactory httpClientFactory, IOptions<ADFSTokenProviderOptions> options, IMemoryCache cache)
        {
            this.options = options.Value;
            this.httpClientFactory = httpClientFactory;
            mcache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<string> AcquireToken()
        {
            Token token = mcache.Get<Token>(tokenkey);
            if (token is null)
            {
                token = await RefreshToken();
            }

            return token.AccessToken;
        }

        private async Task<Token> RefreshToken()
        {
            using var httpClient = httpClientFactory.CreateClient("adfs_token");

            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            // Construct the body of the request
            var pairs = new List<KeyValuePair<string, string>>
                {
                    // TODO : uncomment these three lines and remove hard-coded resource name, clientid, clientsecret
                    new KeyValuePair<string, string>("resource", options.ResourceName),
                    new KeyValuePair<string, string>("client_id", options.ClientId),
                    new KeyValuePair<string, string>("client_secret", options.ClientSecret),
                    //new KeyValuePair<string, string>("resource", "https://embc-dfa.dev.jag.gov.bc.ca/api/data/v9.0/"),
                    //new KeyValuePair<string, string>("client_id", "32ceda11-e0e1-46ee-99b0-83ae3653221d"),
                    //new KeyValuePair<string, string>("client_secret", "rXSIYOGdXEQ82atQ4UPr2-7bpO7Sp8o4Bu_EFMtb"),
                    new KeyValuePair<string, string>("username", $"{options.ServiceAccountDomain}\\{options.ServiceAccountName}"),
                    new KeyValuePair<string, string>("password", options.ServiceAccountPassword),
                    new KeyValuePair<string, string>("scope", "openid"),
                    new KeyValuePair<string, string>("response_mode", "form_post"),
                    new KeyValuePair<string, string>("grant_type", "password")
                };

            try
            {
                // This will also set the content type of the request
                using var content = new FormUrlEncodedContent(pairs);
                // send the request to the ADFS server
                using var response = await httpClient.PostAsync(string.Empty, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                // response should be in JSON format.
                //var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent);
                Token? token = await response.Content.ReadFromJsonAsync<Token>();
                if (token is not null)
                {
                    var options = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(token.ExpiresIn - Buffer) };
                    mcache.Set(tokenkey, token, options);
                    return token;
                }
                else
                {
                    throw new Exception("Unable to get OauthToken from provider");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to obtain access token from {options.OAuth2TokenEndpoint}: {e.Message}", e);
            }
        }
    }

    public class ADFSTokenProviderOptions
    {
        public string OAuth2TokenEndpoint { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ServiceAccountDomain { get; set; }
        public string ServiceAccountName { get; set; }
        public string ServiceAccountPassword { get; set; }
        public string ResourceName { get; set; }
    }

    public static class AccessTokenProviderEx
    {
        public static IServiceCollection AddADFSTokenProvider(this IServiceCollection services)
        {
            services.AddHttpClient("adfs_token", (sp, c) =>
            {
                var options = sp.GetRequiredService<IOptions<ADFSTokenProviderOptions>>().Value;
                c.BaseAddress = new Uri(options.OAuth2TokenEndpoint);
            });
            services.AddTransient<ITokenProvider, ADFSTokenProvider>();

            return services;
        }
    }
}
