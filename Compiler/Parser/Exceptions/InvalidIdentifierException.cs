using System;

namespace Compiler.Parser.Exceptions
{
    public class InvalidIdentifierException : Exception
    {
        private string value { get; set; }

        public override string Message { get; }

        public InvalidIdentifierException(string value)
        {
            this.value = value;
            this.Message = "Unexpected Identifier " + this.value + ".";
        }
    }
}