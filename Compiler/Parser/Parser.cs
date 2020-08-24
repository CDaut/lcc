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

        //A method to check the first token in _tokenList and remove it or raise accoarding error
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

        //The main RDP function
        public Node Parse(NodeType nodeType, bool unopPrecedence = false)
        {
            //declare node to be returned
            Node n;

            //switch over the nodeType to check in which part of the parser we are
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

                    //an expression always has a first term 
                    //this first term is parsed here
                    Node firstTerm = Parse(NodeType.TermNode);
                    n = firstTerm;

                    //get next token
                    Token expressionToken = _tokenList[0];

                    //if the next token is a + or a - it must be a binary operator because we are in an expression
                    //that means that this is not a plain constant but a BinaryOperator node
                    //also check if the last token was a unary operator
                    while ((expressionToken.TokenType == TokenType.AdditionToken ||
                            expressionToken.TokenType == TokenType.NegationToken) && !unopPrecedence)
                    {
                        //remove the - or + token
                        _tokenList.RemoveAt(0);
                        switch (expressionToken.TokenType)
                        {
                            //1. create a BinOp Node
                            //2. Add the first term as a child
                            //3. Parse the rest as an expression and add it as the second term
                            case TokenType.AdditionToken:
                                n = new BinaryOperatorNode(OperatorType.Addition);
                                n.Children.Add(firstTerm);
                                n.Children.Add(Parse(NodeType.ExpressionNode));
                                break;
                            case TokenType.NegationToken:
                                n = new BinaryOperatorNode(OperatorType.Subtraction);
                                n.Children.Add(firstTerm);
                                n.Children.Add(Parse(NodeType.ExpressionNode));
                                break;
                            default:
                                //this should never happen because the while loop checks for token types, that are
                                //handled by case statements above only
                                throw new Exception("WeirdException");
                        }

                        //if there are still tokens left over pop one off
                        if (_tokenList.Count > 0)
                        {
                            expressionToken = _tokenList[0];
                        }
                        else
                        {
                            //there must be tokens left because we are in an expression. 
                            throw new MissingTokenException(TokenType.IntegerLiteralToken);
                        }
                    }

                    break;

                //the case for term node is almost the same as the one for expression node
                case NodeType.TermNode:


                    if (_tokenList.Count == 0)
                    {
                        throw new MissingTokenException(TokenType.IntegerLiteralToken);
                    }

                    //parse first factor
                    Node firstFactor = Parse(NodeType.FactorNode);
                    n = firstFactor;

                    Token termToken = _tokenList[0];

                    //parse second factor if it exists
                    while ((termToken.TokenType == TokenType.MultiplicationToken ||
                            termToken.TokenType == TokenType.DivisionToken) && !unopPrecedence)
                    {
                        _tokenList.RemoveAt(0);
                        switch (termToken.TokenType)
                        {
                            case TokenType.MultiplicationToken:
                                n = new BinaryOperatorNode(OperatorType.Multiplication);
                                n.Children.Add(firstFactor);
                                n.Children.Add(Parse(NodeType.TermNode));
                                break;

                            case TokenType.DivisionToken:
                                n = new BinaryOperatorNode(OperatorType.Division);
                                n.Children.Add(firstFactor);
                                n.Children.Add(Parse(NodeType.TermNode));
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

                    //switch over possible next tokens. There are three possible options 

                    switch (factorToken.TokenType)
                    {
                        //first option:
                        //a parenthesised expression follows.
                        case TokenType.OpenParenthesisToken:
                            //check and remove openParenthesis (it has already been checked in the case, so removal would be enough)
                            CheckFirstTokenAndRemove(TokenType.OpenParenthesisToken);
                            //parse the things inside as an expression
                            n = Parse(NodeType.ExpressionNode);
                            //check if close parenthesis is supplied
                            CheckFirstTokenAndRemove(TokenType.CloseParenthesisToken);
                            break;
                        //second option:
                        //this is a unary operator expression
                        case TokenType.NegationToken:
                        case TokenType.BitwiseComplementToken:
                        case TokenType.LogicalNegationToken:
                            //just parse this as a unary operator node
                            n = Parse(NodeType.UnaryOperatorNode);
                            break;
                        //this is an integer literal.
                        case TokenType.IntegerLiteralToken:
                            //parse it as a constant
                            n = Parse(NodeType.ConstantNode);
                            break;
                        default:
                            throw new UnexpectedTokenException(TokenType.IntegerLiteralToken,
                                factorToken.TokenType);
                    }

                    break;

                //code to parse unary operator nodes
                case NodeType.UnaryOperatorNode:
                    if (_tokenList.Count == 0)
                    {
                        throw new MissingTokenException(TokenType.IntegerLiteralToken);
                    }
                    else
                    {
                        //get operator
                        Token unaryOperator = _tokenList[0];
                        _tokenList.RemoveAt(0);

                        //switch over three different operators and parse the rest as an expression
                        switch (unaryOperator.TokenType)
                        {
                            case TokenType.BitwiseComplementToken:
                                n = new UnaryOperatorNode(OperatorType.BitwiseComplement);
                                n.Children.Add(Parse(NodeType.ExpressionNode, unopPrecedence = true));
                                break;
                            case TokenType.NegationToken:
                                n = new UnaryOperatorNode(OperatorType.Negation);
                                n.Children.Add(Parse(NodeType.ExpressionNode, unopPrecedence = true));
                                break;
                            case TokenType.LogicalNegationToken:
                                n = new UnaryOperatorNode(OperatorType.LogicalNegation);
                                n.Children.Add(Parse(NodeType.ExpressionNode, unopPrecedence = true));
                                break;
                            default:
                                throw new UnexpectedTokenException(TokenType.IntegerLiteralToken,
                                    unaryOperator.TokenType);
                        }
                    }

                    break;

                //parse constant nodes
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

                //default case if the supplied NodeType is unknown
                default:
                    throw new Exception("Unknown Node Type " + nodeType);
            }

            return n;
        }
    }
}