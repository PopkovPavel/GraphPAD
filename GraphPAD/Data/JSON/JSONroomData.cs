using Newtonsoft.Json;

namespace GraphPAD.Data.JSON
{
    public class JSONroomData
    {
        [JsonProperty("userIds")]
        public JSONroomuser[] Users { get; set; }
        
        [JsonProperty("roomOwner")]
        public JSONroomuser RoomOwner { get; set; }
        
        [JsonProperty("_id")]
        public string RoomID { get; set; }

        [JsonProperty("roomName")]
        public string RoomName { get; set; }

        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        [JsonProperty("archiveMessages")]
        public bool ArchiveMessages { get; set; }

        [JsonProperty("inviteCode")]
        public string InviteCode { get; set; }
    }
}
