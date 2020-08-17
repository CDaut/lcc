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
        private readonly List<Token> _tokenList;

        public Parser(List<Token> tokenList)
        {
            _tokenList = tokenList;
        }

        private void CheckFirstTokenAndRemove(TokenType expected)
        {
            if (_tokenList.Count == 0)
            {
                throw new MissingTokenException(expected);
            }

            if (_tokenList[0].TokenType != expected)
            {
                throw new UnexpectedTokenException(expected, _tokenList[0].TokenType);
            }

            _tokenList.RemoveAt(0);
        }

        public Node Parse(NodeType nodeType)
        {
            Node n;
            switch (nodeType)
            {
                case NodeType.ProgramNode:
                    n = new ProgramNode();

                    //if this node is a program node, the next one must be a function node
                    Node childNode = Parse(NodeType.FunctionNode);
                    n.Children.Add(childNode);
                    break;

                case NodeType.FunctionNode:
                    n = new FunctionNode();

                    //check each element of the signature and raise corresponding errors

                    CheckFirstTokenAndRemove(TokenType.IntToken);

                    if (_tokenList.Count == 0)
                    {
                        throw new MissingTokenException(TokenType.IdentifierToken);
                    }

                    if (_tokenList[0].Value != null)
                    {
                        if (_tokenList[0].TokenType == TokenType.IdentifierToken)
                        {
                            ((FunctionNode) n).Name = _tokenList[0].Value.ToString();
                        }
                        else
                        {
                            throw new UnexpectedTokenException(TokenType.IdentifierToken, _tokenList[0].TokenType);
                        }
                    }
                    else
                    {
                        throw new InvalidIdentifierException(null);
                    }

                    //remove <id>
                    _tokenList.RemoveAt(0);

                    CheckFirstTokenAndRemove(TokenType.OpenParenthesisToken);
                    CheckFirstTokenAndRemove(TokenType.CloseParenthesisToken);
                    CheckFirstTokenAndRemove(TokenType.OpenBraceToken);

                    //add returned child node to AST
                    n.Children.Add(Parse(NodeType.StatementNode));

                    //remove trailing }
                    CheckFirstTokenAndRemove(TokenType.CloseBraceToken);

                    break;

                case NodeType.StatementNode:

                    //TODO: This Type of return/statement node will probably need fixing later
                    n = new ReturnNode();

                    //get return token and remove it
                    CheckFirstTokenAndRemove(TokenType.ReturnToken);

                    //add returned child node to AST
                    n.Children.Add(Parse(NodeType.ExpressionNode));


                    //remove trailing ;
                    CheckFirstTokenAndRemove(TokenType.SemicolonToken);


                    break;

                case NodeType.ExpressionNode:

                    if (_tokenList.Count == 0)
                    {
                        throw new MissingTokenException(TokenType.IntegerLiteralToken);
                    }

                    Token constantToken = _tokenList[0];

                    //check if TokenType is right
                    if (constantToken.TokenType != TokenType.IntegerLiteralToken)
                    {
                        throw new UnexpectedTokenException(TokenType.IntToken, constantToken.TokenType);
                    }
                    else
                    {
                        //remove int literal token
                        _tokenList.RemoveAt(0);

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