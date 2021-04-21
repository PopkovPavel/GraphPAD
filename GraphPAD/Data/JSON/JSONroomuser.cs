using Newtonsoft.Json;

namespace GraphPAD.Data.JSON
{
    public class JSONroomuser
    {      
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("nickname")]
        public string Name { get; set; }
    }
}
