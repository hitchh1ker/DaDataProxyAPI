using DaDataProxyAPI.Models.Address;
using DaDataProxyAPI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DaDataProxyAPI.Controllers
{
    public class ProxyController : Controller
    {
        private readonly string apiKey;
        private readonly string apiUrl;
        private readonly string proxyKey;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProxyController(IOptions<ProxySettings> options, IHttpClientFactory httpFactory)
        {
            apiKey = options.Value.ApiKey;
            apiUrl = options.Value.ApiUrl;
            proxyKey = options.Value.ProxyKey;
            _httpClientFactory = httpFactory;
        }

        [HttpGet("/getaddress")]
        public async Task<IActionResult> GetAddressSuggestionsAsync(string address)
        {
            string authorizationHeader = Request.Headers["Authorization"];

            var httpFactory = _httpClientFactory.CreateClient();

            if (authorizationHeader == proxyKey)
            {
                httpFactory.DefaultRequestHeaders.Add("Authorization", $"Token {apiKey}");

                string urlRequest = $"{apiUrl}={address}";

                using var response = await httpFactory.GetAsync(urlRequest);

                string responseBody = await response.Content.ReadAsStringAsync();

                var suggestions = JsonConvert.DeserializeObject<AddressSuggestionsResponse>(responseBody);

                List<string> addressValues = suggestions.Suggestions.Select(x => x.Value).ToList();

                return Ok(addressValues);
            }
            else if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return Forbid();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
