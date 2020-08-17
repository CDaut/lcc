using System;
using Compiler.Lexer;

namespace Compiler.Parser.Exceptions
{
    public class MissingTokenException : Exception
    {
        public override string Message { get; }

        public MissingTokenException(TokenType expected)
        {
            this.Message = "Expected Token " + expected + " missing.";
        }
    }
}