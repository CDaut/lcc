using System;
using Compiler.Lexer;
using Compiler.Parser.Nodes;

namespace Compiler.Generator
{
    public class NotSpecifiedException : Exception
    {
        public override string Message { get; }

        public NotSpecifiedException(NodeType nodeType)
        {
            Message = $"No code generation for {nodeType} specified.";
        }

        public NotSpecifiedException(NodeType nodeType, OperatorType operatorType)
        {
            Message = $"No code generation for {nodeType}:{operatorType} specified.";
        }
    }
}