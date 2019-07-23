using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MineServer.Objects
{
    public class ErrorObject
    {
        [JsonProperty("translate")]
        public string Translate { get; set; }

        [JsonProperty("with")]
        public string[] With { get; set; }
    }
}
