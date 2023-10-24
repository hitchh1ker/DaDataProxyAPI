using DaDataProxyAPI.Models.Address;
using DaDataProxyAPI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DaDataProxyAPI.Controllers
{
    public class ProxyController : Controller
    {
        private readonly IOptions<ProxySettings> _options;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProxyController(IOptions<ProxySettings> options, IHttpClientFactory httpFactory)
        {
            _options = options;
            _httpClientFactory = httpFactory;
        }

        [HttpGet("/getaddress")]
        public async Task<IActionResult> GetAddressSuggestionsAsync(string address)
        {
            string authorizationHeader = Request.Headers["Authorization"];

            using var httpFactory = _httpClientFactory.CreateClient();

            if (authorizationHeader == _options.Value.ProxyKey)
            {
                httpFactory.DefaultRequestHeaders.Add("Authorization", $"Token {_options.Value.ApiKey}");
                string urlRequest = $"{_options.Value.ApiUrl}?query={address}";

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
