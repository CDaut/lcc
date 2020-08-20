using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Compiler.Lexer
{
    class Lexer
    {
        //this method transforms a String into a list of tokens
        public List<Token> Lex(String inputString)
        {
            //strip newlines
            inputString = inputString.Replace("\n", " ");
            inputString = inputString.Replace("\r", " ");

            //initialize Token List
            List<Token> tokens = new List<Token>();

            //while the string is not empty
            while (inputString.Length > 0)
            {
                //trim whitespace
                inputString = inputString.Trim();
                //get next token
                Token t = Next(inputString);
                //append token to list
                tokens.Add(t);
                //remove token from input string
                inputString = inputString.Substring(t.Length);
            }

            return tokens;
        }

        //this function returns the next token from a string
        private Token Next(String input)
        {
            //initialize different possible patterns
            List<Pattern> patterns = new List<Pattern>();

            patterns.Add(new Pattern(@"^\(", TokenType.OpenParenthesisToken));
            patterns.Add(new Pattern(@"^\)", TokenType.CloseParenthesisToken));
            patterns.Add(new Pattern(@"^int ", TokenType.IntToken));
            patterns.Add(new Pattern(@"^}", TokenType.CloseBraceToken));
            patterns.Add(new Pattern(@"^{", TokenType.OpenBraceToken));
            patterns.Add(new Pattern(@"^return ", TokenType.ReturnToken));
            patterns.Add(new Pattern(@"^;", TokenType.SemicolonToken));
            patterns.Add(new Pattern(@"^[a-zA-Z]\w*", TokenType.IdentifierToken));
            patterns.Add(new Pattern(@"^[0-9]+", TokenType.IntegerLiteralToken));
            patterns.Add(new Pattern(@"^-", TokenType.NegationToken));
            patterns.Add(new Pattern(@"^~", TokenType.BitwiseComplementToken));
            patterns.Add(new Pattern(@"^!", TokenType.LogicalNegationToken));

            //try each pattern do determine if it is the one matching at the beginning
            //TODO: There sure is room for optimization here
            foreach (var pattern in patterns)
            {
                //get regex from pattern and match
                Regex r = pattern.ToRegex();
                Match m = r.Match(input);

                //check if the match was successful and at the beginning of the string
                if (m.Index == 0 && m.Success)
                {
                    //generate new token from match
                    Token t = new Token(pattern.GetTokenType());
                    t.Length = m.Length;
                    t.Value = null;

                    //switch over token types to assign a value if needed
                    //the default case "InvalidToken" is here for security and should never happen. If it does, 
                    //something is wrong with the regex
                    switch (pattern.GetTokenType())
                    {
                        case TokenType.IntegerLiteralToken:
                            //TODO: Error handling is missing here
                            t.Value = Int32.Parse(m.Value);
                            break;
                        case TokenType.IdentifierToken:
                            t.Value = m.Value;
                            break;
                        case TokenType.ReturnToken:
                        case TokenType.CloseParenthesisToken:
                        case TokenType.OpenParenthesisToken:
                        case TokenType.OpenBraceToken:
                        case TokenType.CloseBraceToken:
                        case TokenType.IntToken:
                        case TokenType.SemicolonToken:
                        case TokenType.NegationToken:
                        case TokenType.BitwiseComplementToken:
                        case TokenType.LogicalNegationToken:
                        case TokenType.InvalidToken:
                            break;
                        default:
                            t.TokenType = TokenType.InvalidToken;
                            throw new Exception("Match found, but no corresponding token. Check regex!");
                    }

                    return t;
                }
            }

            //return the next token
            return new Token(TokenType.InvalidToken);
        }
    }
}