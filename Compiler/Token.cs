using System;

namespace Compiler
{
    public class Token
    {
        public TokenType TokenType { get;  set; }
        public String Value { get; set; }
        public int Length { get; set;  }

        public Token(TokenType pTokenType)
        {
            this.TokenType = pTokenType;
        }

        public override string ToString()
        {
            if (Value == null)
            {
                return TokenType.ToString();
            }
            else
            {
                return TokenType.ToString() + ":" + Value.ToString();
            }
        }
    }
}