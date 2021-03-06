using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPAD.Data.JSON
{
    public class JSONroomData
    {
        [JsonProperty("userIds")]
        public string[] Users { get; set; }

        [JsonProperty("_id")]
        public string RoomID { get; set; }

        [JsonProperty("roomOwner")]
        public string RoomOwner { get; set; }

        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        [JsonProperty("archiveMessages")]
        public bool ArchiveMessages { get; set; }

        [JsonProperty("inviteCode")]
        public string InviteCode { get; set; }
    }
}
