using System;

namespace Compiler.Parser.Exceptions
{
    public class WrongTypeException : Exception
    {
        private Type expected;
        private Type got;

        public override string Message { get; }

        public WrongTypeException(Type expected, Type got)
        {
            this.expected = expected;
            this.got = got;

            this.Message = "Expected type " + expected +
                           "but got type " + got + ".";
        }
    }
}