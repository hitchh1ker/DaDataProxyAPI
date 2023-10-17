namespace DaDataProxyAPI.Models.Address
{
    public class AddressSuggestionsRequest
    {
        public List<AddressSuggestionRequest> Suggestions { get; set; }
    }

    public class AddressSuggestionRequest
    {
        public string Value { get; set; }
    }
}
