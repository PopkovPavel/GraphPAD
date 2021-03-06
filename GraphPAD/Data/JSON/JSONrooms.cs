using Newtonsoft.Json;

namespace GraphPAD.Data.JSON
{
    public class JSONrooms
    {
        [JsonProperty("data")]
        public JSONroomData[] Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }


    }
}
