using System.Collections.Generic;

namespace Compiler.Parser.Nodes
{
    public abstract class Node
    {
        public abstract NodeType NodeType { get; set; }
        public List<Node> Children { get; set; }

        protected Node()
        {
            Children = new List<Node>();
        }
    }
}