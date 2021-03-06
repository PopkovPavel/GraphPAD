using Newtonsoft.Json;

namespace GraphPAD.Data.JSON
{
    public class JSONuserInfo
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("userId")]
        public string userId { get; set; }

        [JsonProperty("nickname")]
        public string Name { get; set; }
    }
}
