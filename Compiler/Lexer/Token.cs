using System;
using System.Linq.Expressions;

namespace Compiler.Lexer
{
    public class Token
    {
        public TokenType TokenType { get;  set; }
        public Object Value { get; set; }
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