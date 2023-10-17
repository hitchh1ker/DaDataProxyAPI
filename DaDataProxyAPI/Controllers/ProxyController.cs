using DaDataProxyAPI.Models.Address;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DaDataProxyAPI.Controllers
{
    [ApiController]
    [Route("/proxy")]
    public class ProxyController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public ProxyController(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        [HttpGet]
        [Route("/getaddress")]
        public async Task<IActionResult> GetAddressSuggestions(string address)
        {
            try
            {
                string apiUrl = $"https://suggestions.dadata.ru/suggestions/api/4_1/rs/suggest/address?query={address}";

                string proxyKey = _configuration.GetSection("ProxyKey:ProxyKey").Value;
                string proxyHeader = Request.Headers["Proxy-Authorization"];

                string apiKey = _configuration.GetSection("DaDataConfig:ApiKey").Value;
                string authorizationHeader = Request.Headers["Authorization"];

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {authorizationHeader}");

                if (proxyHeader == proxyKey)
                {
                    if (authorizationHeader == apiKey)
                    {
                        HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                        string responseBody = await response.Content.ReadAsStringAsync();

                        var suggestions = JsonConvert.DeserializeObject<AddressSuggestionsRequest>(responseBody);

                        List<string> addressValues = suggestions.Suggestions.Select(s => s.Value).ToList();

                        return Ok(addressValues);
                    }
                    else if (string.IsNullOrEmpty(authorizationHeader))
                    {
                        return Forbid();
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
