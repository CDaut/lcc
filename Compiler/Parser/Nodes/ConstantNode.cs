namespace Compiler.Parser.Nodes
{
    public sealed class ConstantNode : Node
    {
        public override NodeType NodeType { get; set; }
        public int value { get; set; }

        public ConstantNode(int value)
        {
            this.NodeType = NodeType.ConstantNode;
            this.value = value;
        }
    }
}