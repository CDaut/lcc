namespace Compiler.Parser.Nodes
{
    public sealed class ProgramNode : Node
    {
        public override NodeType NodeType { get; set; }

        public ProgramNode()
        {
            this.NodeType = NodeType.ProgramNode;
        }
    }
}