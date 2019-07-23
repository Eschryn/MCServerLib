using MineServer.Converters;
using MineServer.MCFormat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineServer.Objects
{
    public class ChatObject
    {
#pragma warning disable IDE0052 // Ungelesene private Member entfernen
        private string rawTxt;
#pragma warning restore IDE0052 // Ungelesene private Member entfernen
        private string txt;
        [JsonProperty("text")]
        public string Text
        {
            set
            {
                if (!(value.Contains('&') || value.Contains('§')))
                {
                    rawTxt = txt = value;
                    return;
                }

                var tokenStream = TokenStream.FromString(value);
                var fp = new FormatParser(tokenStream);
                var co = fp.Parse();
                Bold |= co.Bold;
                Italic |= co.Italic;
                Underlined |= co.Underlined;
                Strikethrough |= co.Strikethrough;
                Obfuscated |= co.Obfuscated;
                Color = co.Color ?? Color;
                if (Children is null)
                    Children = co.Children;
                else
                    Children = Children.Concat(co.Children).ToArray();
                txt = co.Text;
                rawTxt = value;
            }
            get
            {
                if (txt is null)
                    return "";
                return txt;
            }
        }

        [JsonProperty("bold")]
        public bool Bold { get; set; }

        [JsonProperty("italic")]
        public bool Italic { get; set; }

        [JsonProperty("underlined")]
        public bool Underlined { get; set; }

        [JsonProperty("strikethrough")]
        public bool Strikethrough { get; set; }

        [JsonProperty("obfuscated")]
        public bool Obfuscated { get; set; }

        [JsonProperty("color")]
        [JsonConverter(typeof(ColorConverter))]
        public MineColor Color { get; set; }

        public void WriteToConsole(bool newLine = false)
        {
            Console.ForegroundColor = MineColor.White.ConsoleColor;
            WriteToConsoleInternal();
            if (newLine)
                Console.WriteLine();
        }
        private void WriteToConsoleInternal()
        {
            if (!(Color is null))
                Console.ForegroundColor = Color.ConsoleColor;
            if (!(Text is null))
                Console.Write(Text);

            if (Children is null)
                return;

            foreach (var e in Children)
            {
                if (!(e is null))
                    e.WriteToConsoleInternal();
            }
        }

        [JsonProperty("extra")]
        public ChatObject[] Children { get; set; }

        public override string ToString() => $"{Text}";
    }
}
