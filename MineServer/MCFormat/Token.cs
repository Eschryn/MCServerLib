namespace MineServer.MCFormat
{
    public class Token
    {
        public static Token EOF { get; } = new Token { Type = TokenType.EOF };

        public TokenType Type;
        public string Value;

        public override string ToString() => $"[{Type}] {Value}";
    }
}