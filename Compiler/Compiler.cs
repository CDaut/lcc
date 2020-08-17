using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Compiler.Lexer;
using Compiler.Parser;
using Compiler.Parser.Nodes;

namespace Compiler
{
    public static class Compiler
    {
        public static void Main()
        {
            string[] validFiles = Directory.GetFiles("../../../../tests/week_1/valid");
            string[] invalidFiles = Directory.GetFiles("../../../../tests/week_1/invalid");

            foreach (string filepath in validFiles)
            {
                List<Token> tokens = TestLexer(filepath, 0);
                TestParser(tokens, filepath, 1);
            }

            foreach (string filepath in invalidFiles)
            {
                List<Token> tokens = TestLexer(filepath, 0);
                TestParser(tokens, filepath, 1);
            }
        }

        static void PrettyPrint(Node root, string indent)
        {
            Console.WriteLine(indent + root.NodeType);
            foreach (Node child in root.Children)
            {
                PrettyPrint(child, indent + "    ");
            }
        }

        static List<Token> TestLexer(string path, int debugLevel)
        {
            Lexer.Lexer lexer = new Lexer.Lexer();


            StreamReader file = new StreamReader(path);
            string contents = file.ReadToEnd();

            List<Token> tokens = lexer.Lex(contents);
            Console.WriteLine("Lexed \"" + path.Split("/").Last() + "\"");

            if (debugLevel > 0)
            {
                Console.WriteLine("-----------" + path + "-----------");
                foreach (Token token in tokens)
                {
                    Console.WriteLine(token.ToString());
                }

                Console.WriteLine("--------------------------------------");
            }


            return tokens;
        }

        static void TestParser(List<Token> tokenList, string path, int debugLevel)
        {
            Parser.Parser p = new Parser.Parser(tokenList);

            try
            {
                Node programNode = p.Parse(NodeType.ProgramNode);
                Console.WriteLine("Parsed \"" + path.Split("/").Last() + "\"");
                if (debugLevel > 0)
                {
                    PrettyPrint(programNode, "");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in file \"" + path.Split("/").Last() + "\"");
                Console.WriteLine(e.Message);
            }
        }
    }
}