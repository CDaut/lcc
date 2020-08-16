using System;

namespace Compiler.Parser.Exceptions
{
    public class MissingSemicolonException : Exception
    {
        public override string Message { get; }
    }
}