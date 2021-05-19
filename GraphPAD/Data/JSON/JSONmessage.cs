using Newtonsoft.Json;


namespace GraphPAD.Data.JSON
{
    class JSONmessage
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}
