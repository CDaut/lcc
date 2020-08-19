using System;
using System.Linq;
using Compiler.Parser;
using Compiler.Parser.Nodes;

namespace Compiler.Generator
{
    public class Generator
    {
        public string Generate(Node rootNode)
        {
            string s = "";
            switch (rootNode.NodeType)
            {
                case NodeType.ProgramNode:
                    foreach (Node rootNodeChild in rootNode.Children)
                    {
                        s += Generate(rootNodeChild);
                    }

                    break;
                case NodeType.FunctionNode:
                    string identifier = ((FunctionNode) rootNode).Name;
                    string functionBody = "";
                    foreach (Node rootNodeChild in rootNode.Children)
                    {
                        functionBody += Generate(rootNodeChild);
                    }

                    s = $".globl {identifier}\n" +
                        $"{identifier}:\n" +
                        $"{functionBody}";
                    break;
                case NodeType.ReturnStatementNode:
                    s = $"movl ${Generate(rootNode.Children[0])}, %eax \n" +
                        "ret\n";
                    break;
                case NodeType.ExpressionNode:
                    s = ((ConstantNode) rootNode).value.ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return s;
        }
    }
}