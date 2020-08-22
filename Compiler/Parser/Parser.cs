using System;
using System.Collections.Generic;
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
                    n.Children.Add(Parse(NodeType.ReturnStatementNode));

                    //remove trailing }
                    CheckFirstTokenAndRemove(TokenType.CloseBraceToken);

                    break;

                case NodeType.ReturnStatementNode:

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

                    Node firstTerm = Parse(NodeType.TermNode);
                    n = firstTerm;

                    Token expressionToken = _tokenList[0];

                    while (expressionToken.TokenType == TokenType.AdditionToken ||
                           expressionToken.TokenType == TokenType.NegationToken)
                    {
                        _tokenList.RemoveAt(0);
                        switch (expressionToken.TokenType)
                        {
                            case TokenType.AdditionToken:
                                n = new BinaryOperatorNode(OperatorType.Addition);
                                n.Children.Add(firstTerm);
                                n.Children.Add(Parse(NodeType.TermNode));
                                break;
                            case TokenType.NegationToken:
                                n = new BinaryOperatorNode(OperatorType.Subtraction);
                                n.Children.Add(firstTerm);
                                n.Children.Add(Parse(NodeType.TermNode));
                                break;
                            default:
                                throw new Exception("WeirdException");
                        }

                        if (_tokenList.Count > 0)
                        {
                            expressionToken = _tokenList[0];
                        }
                        else
                        {
                            throw new MissingTokenException(TokenType.IntegerLiteralToken);
                        }
                    }

                    break;

                case NodeType.TermNode:


                    if (_tokenList.Count == 0)
                    {
                        throw new MissingTokenException(TokenType.IntegerLiteralToken);
                    }

                    Node firstFactor = Parse(NodeType.FactorNode);
                    n = firstFactor;

                    Token termToken = _tokenList[0];

                    while (termToken.TokenType == TokenType.MultiplicationToken ||
                           termToken.TokenType == TokenType.DivisionToken)
                    {
                        _tokenList.RemoveAt(0);
                        switch (termToken.TokenType)
                        {
                            case TokenType.MultiplicationToken:
                                n = new BinaryOperatorNode(OperatorType.Multiplication);
                                n.Children.Add(firstFactor);
                                n.Children.Add(Parse(NodeType.FactorNode));
                                break;

                            case TokenType.DivisionToken:
                                n = new BinaryOperatorNode(OperatorType.Division);
                                n.Children.Add(firstFactor);
                                n.Children.Add(Parse(NodeType.FactorNode));
                                break;
                            default:
                                throw new Exception("WeirdException");
                        }

                        if (_tokenList.Count > 0)
                        {
                            termToken = _tokenList[0];
                        }
                        else
                        {
                            throw new MissingTokenException(TokenType.IntegerLiteralToken);
                        }
                    }

                    break;

                case NodeType.FactorNode:
                    if (_tokenList.Count == 0)
                    {
                        throw new MissingTokenException(TokenType.IntegerLiteralToken);
                    }

                    Token factorToken = _tokenList[0];

                    switch (factorToken.TokenType)
                    {
                        case TokenType.OpenParenthesisToken:
                            CheckFirstTokenAndRemove(TokenType.OpenParenthesisToken);
                            n = Parse(NodeType.ExpressionNode);
                            CheckFirstTokenAndRemove(TokenType.CloseParenthesisToken);
                            break;
                        case TokenType.NegationToken:
                        case TokenType.BitwiseComplementToken:
                        case TokenType.LogicalNegationToken:
                            n = Parse(NodeType.UnaryOperatorNode);
                            n.Children.Add(Parse(NodeType.FactorNode));
                            break;
                        case TokenType.IntegerLiteralToken:
                            n = Parse(NodeType.ConstantNode);
                            break;
                        default:
                            throw new UnexpectedTokenException(TokenType.IntegerLiteralToken,
                                factorToken.TokenType);
                    }

                    break;

                case NodeType.UnaryOperatorNode:
                    if (_tokenList.Count == 0)
                    {
                        throw new MissingTokenException(TokenType.IntegerLiteralToken);
                    }
                    else
                    {
                        Token unaryOperator = _tokenList[0];
                        _tokenList.RemoveAt(0);

                        switch (unaryOperator.TokenType)
                        {
                            case TokenType.BitwiseComplementToken:
                                n = new UnaryOperatorNode(OperatorType.BitwiseComplement);
                                n.Children.Add(Parse(NodeType.ExpressionNode));
                                break;
                            case TokenType.NegationToken:
                                n = new UnaryOperatorNode(OperatorType.Negation);
                                n.Children.Add(Parse(NodeType.ExpressionNode));
                                break;
                            case TokenType.LogicalNegationToken:
                                n = new UnaryOperatorNode(OperatorType.LogicalNegation);
                                n.Children.Add(Parse(NodeType.ExpressionNode));
                                break;
                            default:
                                throw new UnexpectedTokenException(TokenType.IntegerLiteralToken,
                                    unaryOperator.TokenType);
                        }
                    }

                    break;

                case NodeType.ConstantNode:

                    if (_tokenList.Count == 0)
                    {
                        throw new MissingTokenException(TokenType.IntegerLiteralToken);
                    }

                    //double check, for safety. Pbly unnecesarry
                    if (_tokenList[0].TokenType != TokenType.IntegerLiteralToken)
                    {
                        throw new UnexpectedTokenException(TokenType.IntegerLiteralToken, _tokenList[0].TokenType);
                    }
                    else
                    {
                        //return final constant node to end recursion
                        n = new ConstantNode((int) _tokenList[0].Value);
                        _tokenList.RemoveAt(0);
                        break;
                    }

                default:
                    throw new Exception("Unknown Node Type " + nodeType);
            }

            return n;
        }
    }
}