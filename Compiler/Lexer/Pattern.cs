using System;
using System.Text.RegularExpressions;

namespace Compiler.Lexer
{
    public class Pattern
    {
        private String pattern;
        private TokenType tokenType;

        public Pattern(String pattern, TokenType tokenType)
        {
            this.pattern = pattern;
            this.tokenType = tokenType;
        }

        public Regex ToRegex()
        {
            return new Regex(this.pattern);
        }

        public TokenType GetTokenType()
        {
            return this.tokenType;
        }
    }
}