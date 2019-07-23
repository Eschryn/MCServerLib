using MineServer.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MineServer.Objects
{
    public class SLPObject
    {
        [JsonProperty("description")]
        public ChatObject Description { get; set; }

        [JsonProperty("players")]
        public Players Players { get; protected set; }

        [JsonProperty("version")]
        public Version Version { get; protected set; }

        [JsonProperty("favicon")]
        [JsonConverter(typeof(FaviconConverter))]
        public byte[] Favicon { get; protected set; }

        public override string ToString() => Description.Text;
    }

    public class PlayerSample
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public override string ToString() => Name;
    }


    public class Players
    {
        [JsonProperty("max")]
        public int Max { get; protected set; }

        [JsonProperty("online")]
        public int Online { get; protected set; }

        [JsonProperty("sample")]
        public PlayerSample[] Sample { get; protected set; }

        public override string ToString() => $"{Online}/{Max}";
    }

    public class Version
    {
        [JsonProperty("name")]
        public string Name { get; protected set; }

        [JsonProperty("protocol")]
        public int Protocol { get; protected set; }

        public override string ToString() => $"{Name}; {Protocol}";
    }

}
