using Newtonsoft.Json;

namespace consoleApplication
{
    public class BreakingBadQuote
    {
        [JsonProperty("quote_id")]
        public int quote_id { get; set; }
        [JsonProperty("quote")]
        public string quote { get; set; }
        [JsonProperty("author")]
        public string author { get; set; }
        [JsonProperty("series")]
        public string series { get; set; }
    }
}
