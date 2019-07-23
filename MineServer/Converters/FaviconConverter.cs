using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MineServer.Converters
{
    public class FaviconConverter : JsonConverter<byte[]>
    {
        public override byte[] ReadJson(JsonReader reader, Type objectType, byte[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is string s)
                return Convert.FromBase64String(s.Substring(22));

            throw new FormatException("Invalid Favicon");
        }

        public override void WriteJson(JsonWriter writer, byte[] value, JsonSerializer serializer)
        {
            writer.WriteValue(Convert.ToBase64String(value));
        }
    }
}
