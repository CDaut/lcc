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
                    s = $"{Generate(rootNode.Children[0])}" +
                        "ret\n";
                    break;
                case NodeType.ExpressionNode:
                    s = Generate(rootNode.Children[0]);
                    break;
                case NodeType.ConstantNode:
                    s = $"movl ${((ConstantNode) rootNode).value.ToString()}, %eax\n";
                    break;
                case NodeType.UnaryOperatorNode:
                    switch (((UnaryOperatorNode) rootNode).OperatorType)
                    {
                        case OperatorType.Negation:
                            s = $"{Generate(rootNode.Children[0])}" +
                                "neg %eax\n";
                            break;
                        case OperatorType.BitwiseComplement:
                            s = $"{Generate(rootNode.Children[0])}" +
                                "not %eax\n";
                            break;
                        case OperatorType.LogicalNegation:
                            s = $"{Generate(rootNode.Children[0])}" +
                                "cmpl $0, %eax\n" +
                                "movl $0, %eax\n" + //xorl %eax, %eax should also work, but doesn't
                                "sete %al\n";
                            break;
                        default:
                            throw new NotSpecifiedException(NodeType.UnaryOperatorNode,
                                ((UnaryOperatorNode) rootNode).OperatorType);
                    }

                    break;
                case NodeType.BinaryOperatorNode:
                    switch (((BinaryOperatorNode) rootNode).OperatorType)
                    {
                        case OperatorType.Addition:
                            s = $"{Generate(rootNode.Children[0])}" +
                                "push %rax\n" +
                                $"{Generate(rootNode.Children[1])}" +
                                "pop %rcx\n" +
                                "add %ecx, %eax\n";
                            break;
                        case OperatorType.Subtraction:
                            s = $"{Generate(rootNode.Children[1])}" +
                                "push %rax\n" +
                                $"{Generate(rootNode.Children[0])}" +
                                "pop %rcx\n" +
                                "sub %ecx, %eax\n";
                            break;
                        case OperatorType.Multiplication:
                            s = $"{Generate(rootNode.Children[0])}" +
                                "push %rax\n" +
                                $"{Generate(rootNode.Children[1])}" +
                                "pop %rcx\n" +
                                "imul %rcx, %rax\n";
                            break;
                        case OperatorType.Division:
                            s = $"{Generate(rootNode.Children[0])}" +
                                "push %rax\n" +
                                $"{Generate(rootNode.Children[1])}" +
                                "movl %eax, %ecx\n" + //move calculated divisor to %ecx
                                "pop %rax\n" + //pop divident do %eax
                                "cdq\n" +
                                "divl %ecx\n" +
                                "movl %ecx, %eax\n";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                default:
                    throw new NotSpecifiedException(rootNode.NodeType);
            }

            return s;
        }
    }
}