using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.Lexer;
using Compiler.Parser.Exceptions;
using Compiler.Parser.Nodes;

namespace Compiler.Parser
{
    public class Parser
    {
        public Node Parse(ref List<Token> tokenList, NodeType nodeType)
        {
            var t = tokenList.FirstOrDefault();

            if (t == null) throw new Exception("Empty Token List.");
            Node n;
            switch (nodeType)
            {
                case NodeType.ProgramNode:
                    n = new ProgramNode();

                    //if this node is a program node, the next one must be a function node
                    Node childNode = Parse(ref tokenList, NodeType.FunctionNode);
                    n.Children.Add(childNode);
                    break;

                case NodeType.FunctionNode:
                    n = new FunctionNode();

                    //retrieve signature and remove it
                    List<Token> signature = tokenList.GetRange(0, 5);
                    tokenList.RemoveRange(0, 5);

                    //check each element of the signature and raise corresponding errors
                    if (signature[0].TokenType != TokenType.IntToken)
                    {
                        throw new UnexpectedTokenException(TokenType.IntToken, signature[1].TokenType);
                    }

                    if (signature[1].Value != null)
                    {
                        if (signature[1].TokenType == TokenType.IdentifierToken)
                        {
                            ((FunctionNode) n).Name = signature[1].Value.ToString();
                        }
                        else
                        {
                            throw new UnexpectedTokenException(TokenType.IdentifierToken, signature[1].TokenType);
                        }
                    }
                    else
                    {
                        throw new InvalidIdentifierException(null);
                    }

                    if (signature[2].TokenType != TokenType.OpenParenthesisToken)
                    {
                        throw new UnexpectedTokenException(TokenType.OpenParenthesisToken, signature[1].TokenType);
                    }

                    if (signature[3].TokenType != TokenType.CloseParenthesisToken)
                    {
                        throw new UnexpectedTokenException(TokenType.CloseParenthesisToken, signature[1].TokenType);
                    }

                    if (signature[4].TokenType != TokenType.OpenBraceToken)
                    {
                        throw new UnexpectedTokenException(TokenType.OpenBraceToken, signature[1].TokenType);
                    }

                    //add returned child node to AST
                    n.Children.Add(Parse(ref tokenList, NodeType.StatementNode));

                    //remove trailing }
                    if (tokenList[0].TokenType != TokenType.CloseBraceToken)
                    {
                        throw new UnexpectedTokenException(TokenType.CloseBraceToken, tokenList[0].TokenType);
                    }

                    tokenList.RemoveAt(0);
                    break;
                case NodeType.StatementNode:

                    //TODO: This Type of return/statement node will probably need fixing later
                    n = new ReturnNode();

                    //get return token and remove it
                    List<Token> returnStatement = tokenList.GetRange(0, 1);
                    tokenList.RemoveAt(0);

                    if (returnStatement[0].TokenType != TokenType.ReturnToken)
                    {
                        throw new UnexpectedTokenException(TokenType.ReturnToken, returnStatement[0].TokenType);
                    }
                    else
                    {
                        //add returned child node to AST
                        n.Children.Add(Parse(ref tokenList, NodeType.ExpressionNode));
                        //remove trailing ;

                        if (tokenList[0].TokenType != TokenType.SemicolonToken)
                        {
                            throw new UnexpectedTokenException(TokenType.SemicolonToken, tokenList[0].TokenType);
                        }

                        tokenList.RemoveAt(0);
                    }

                    break;

                case NodeType.ExpressionNode:

                    Token constantToken = tokenList[0];
                    //check if TokenType is right
                    if (constantToken.TokenType != TokenType.IntegerLiteralToken)
                    {
                        throw new UnexpectedTokenException(TokenType.IntToken, constantToken.TokenType);
                    }
                    else
                    {
                        //check if value Type is right
                        if (constantToken.Value.GetType() != typeof(int))
                        {
                            throw new WrongTypeException(typeof(int), constantToken.Value.GetType());
                        }

                        //return final constant node to end recursion
                        n = new ConstantNode((int) constantToken.Value);
                    }

                    break;

                default:
                    throw new Exception("Unknown Node Type " + nodeType);
            }

            return n;
        }
    }
}