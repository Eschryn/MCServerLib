using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MineServer.MCFormat
{
    public class TokenStream
    {
        private readonly BinaryReader read;
        private readonly char[] fmtTokens = "§&".ToArray();
        private Token peek = null; 

        public TokenStream(Stream stream)
        {
            this.BaseStream = stream;
            read = new BinaryReader(stream, Encoding.UTF8);
        }

        public bool EOF { get => BaseStream.Length == BaseStream.Position; }

        private string ReadTo(params char[] indicators)
        {
            var sb = new List<char>();
            while (!EOF && !indicators.Contains((char)read.PeekChar()))
                sb.Add(read.ReadChar());
            return new string(sb.ToArray());
        }

        public Token NextToken()
        {
            // HACK: fix try catch control
            if (!(peek is null))
                try { return peek; } 
                finally { peek = null; }

            switch ((char)read.PeekChar())
            {
                case char c when fmtTokens.Contains(c):
                    return new Token { Type = TokenType.Format, Value = read.ReadChars(2)[1].ToString() };
                default:
                    var str = ReadTo(fmtTokens);
                    if (!string.IsNullOrEmpty(str))
                        return new Token { Type = TokenType.String, Value = str };

                    if (!EOF)
                        return NextToken();

                    return null;
            }
        }

        public Token Peek()
        {
            return peek = NextToken();
        }

        public static TokenStream FromString(string str) => FromString(str, Encoding.Default);
        public static TokenStream FromString(string str, Encoding encoding)
        {
            return new TokenStream(new MemoryStream(encoding.GetBytes(str)));
        }

        public Stream BaseStream { get; }
    }
}
