namespace Compiler.Parser.Nodes
{
    public sealed class BinaryOperatorNode : Node
    {
        public override NodeType NodeType { get; set; }

        public OperatorType OperatorType { get; set; }

        public BinaryOperatorNode(OperatorType operatorType)
        {
            NodeType = NodeType.BinaryOperatorNode;
            OperatorType = operatorType;
        }
    }
}