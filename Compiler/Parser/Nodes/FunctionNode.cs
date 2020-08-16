using System;

namespace Compiler.Parser.Nodes
{
    public sealed class FunctionNode : Node
    {
        public string Name { get; set; }
        public override NodeType NodeType { get; set; }

        public FunctionNode()
        {
            this.NodeType = NodeType.FunctionNode;
        }
    }
}