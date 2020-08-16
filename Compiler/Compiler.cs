using System;
using System.Collections.Generic;
using System.IO;
using Compiler.Lexer;
using Compiler.Parser;

namespace Compiler
{
    public class Compiler
    {
        public static void Main()
        {
            List<List<Token>> tokensValid = TestLexer("../../../../tests/week_1/valid");
            List<List<Token>> tokensInvalid = TestLexer("../../../../tests/week_1/invalid");

            TestParser(tokensValid[0]);
            TestParser(tokensInvalid[0]);
        }

        static List<List<Token>> TestLexer(String path)
        {
            List<List<Token>> tokenLists = new List<List<Token>>();
            String[] files = Directory.GetFiles(path);
            Lexer.Lexer lexer = new Lexer.Lexer();

            foreach (String filename in files)
            {
                StreamReader file = new StreamReader(filename);
                String contents = file.ReadToEnd();

                List<Token> tokens = lexer.Lex(contents);
                tokenLists.Add(tokens);

                Console.WriteLine("-----------" + filename + "-----------");
                foreach (Token token in tokens)
                {
                    Console.WriteLine(token.ToString());
                }

                Console.WriteLine("--------------------------------------");
            }

            return tokenLists;
        }

        static void TestParser(List<Token> tokenList)
        {
            Parser.Parser p = new Parser.Parser();

            p.Parse(ref tokenList, NodeType.ProgramNode);
        }
    }
}