namespace Compiler.Parser.Nodes
{
    public sealed class ReturnNode : Node
    {
        public override NodeType NodeType { get; set; }

        public ReturnNode()
        {
            this.NodeType = NodeType.StatementNode;
        }
    }
}