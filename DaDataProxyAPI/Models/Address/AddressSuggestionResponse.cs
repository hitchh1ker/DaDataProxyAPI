namespace DaDataProxyAPI.Models.Address
{
    public class AddressSuggestionsResponse
    {
        public List<AddressSuggestionResponse> Suggestions { get; set; }
    }
    public class AddressSuggestionResponse
    {
        public string Value { get; set; }
    }
}
