using System;
using Compiler.Lexer;

namespace Compiler.Parser.Exceptions
{
    public class UnexpectedTokenException : Exception
    {
        public override string Message { get; }

        public UnexpectedTokenException(TokenType expected, TokenType got)
        {
            this.Message = "Unexpected Token " + got + ", expected: " + expected + ".";
        }
    }
}