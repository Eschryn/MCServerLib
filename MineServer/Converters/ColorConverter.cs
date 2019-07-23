using MineServer.MCFormat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MineServer.Converters
{
    public class ColorConverter : JsonConverter<MineColor>
    {
        public override MineColor ReadJson(JsonReader reader, Type objectType, MineColor existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            switch (reader.Value)
            {
                case char c: 
                    return MineColor.Find(c);
                case string s:
                    return MineColor.Find(s);
            }

            throw new FormatException("Invalid Color");
        }

        public override void WriteJson(JsonWriter writer, MineColor value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }
    }
}
