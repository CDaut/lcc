namespace Compiler.Parser.Nodes
{
    public sealed class UnaryOperatorNode : Node
    {
        public override NodeType NodeType { get; set; }
        public OperatorType OperatorType { get; set; }

        public UnaryOperatorNode(OperatorType operatorType)
        {
            OperatorType = operatorType;
            NodeType = NodeType.UnaryOperatorNode;
        }
    }
}