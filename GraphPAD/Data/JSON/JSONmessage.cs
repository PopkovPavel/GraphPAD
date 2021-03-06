using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPAD.Data.JSON
{
    class JSONmessage
    {
        [JsonProperty("message")]
        public string Message;
        [JsonProperty("userId")]
        public string UserId;
    }
}
