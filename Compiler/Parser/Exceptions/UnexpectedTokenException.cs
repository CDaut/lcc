using System;
using Compiler.Lexer;

namespace Compiler.Parser.Exceptions
{
    public class UnexpectedTokenException : Exception
    {
        public TokenType expected { get; set; }
        public TokenType got { get; set; }

        public override string Message { get; }

        public UnexpectedTokenException(TokenType expected, TokenType got)
        {
            this.expected = expected;
            this.got = got;
            this.Message = "Unexpected Token " + got + ". Expected: " + expected;
        }
    }
}