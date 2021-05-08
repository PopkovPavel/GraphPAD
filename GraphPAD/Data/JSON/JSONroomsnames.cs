using Newtonsoft.Json;
namespace GraphPAD.Data.JSON
{
    class JSONroomsnames
    {
        [JsonProperty("rooms")]
        public JSONroomname [] jSONroomnames { get; set; }
    }
}
