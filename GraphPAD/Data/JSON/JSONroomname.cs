using Newtonsoft.Json;

namespace GraphPAD.Data.JSON
{
    class JSONroomname
    {

        [JsonProperty("roomId")]
        public string RoomID { get; set; }

        [JsonProperty("roomName")]
        public string RoomName { get; set; }
    }
}
