using Newtonsoft.Json;

namespace GraphPAD.Data.JSON
{
    public class JSONauth
    {
        [JsonProperty("data")]
        public JSONuserInfo Data { get; set; }

        [JsonProperty("accessToken")]
        public string Token { get; set; }
    }
}
