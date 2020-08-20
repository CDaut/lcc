using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Compiler.Lexer;
using Compiler.Parser.Nodes;

namespace Compiler
{
    public static class DevFunctions
    {
        static void PrettyPrint(Node root, string indent)
        {
            switch (root.NodeType)
            {
                case NodeType.FunctionNode:
                    Console.WriteLine(indent + root.NodeType + ":" + ((FunctionNode) root).Name);
                    break;
                case NodeType.ConstantNode:
                    Console.WriteLine(indent + root.NodeType + ":" + ((ConstantNode) root).value);
                    break;
                case NodeType.UnaryOperatorNode:
                    Console.WriteLine(indent + root.NodeType + ":" + ((UnaryOperatorNode) root).OperatorType);
                    break;

                default:
                    Console.WriteLine(indent + root.NodeType);
                    break;
            }

            foreach (Node child in root.Children)
            {
                PrettyPrint(child, indent + "    ");
            }
        }

        static void TestGenerator(Node root, int debugLevel)
        {
            if (root != null)
            {
                Generator.Generator gen = new Generator.Generator();
                string asm = gen.Generate(root);

                if (debugLevel > 0)
                {
                    Console.Write(asm);
                }
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
                foreach (Token token in tokens)
                {
                    Console.WriteLine(token.ToString());
                }
            }


            return tokens;
        }

        static Node TestParser(List<Token> tokenList, string path, int debugLevel)
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

                return programNode;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in file \"" + path.Split("/").Last() + "\"");
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public static void DevMode()
        {
            for (int i = 1; i <= 2; i++)
            {
                Console.WriteLine($"---------------------valid, stage {i}-------------------------------");
                foreach (string file in Directory.GetFiles($"/home/clemens/repositorys/lcc/stage_{i}/valid"))
                {
                    Console.WriteLine("-------------");
                    List<Token> tokens = TestLexer(file, 0);
                    Node programNode = TestParser(tokens, file, 1);
                    TestGenerator(programNode, 1);
                }

                /*
                Console.WriteLine($"---------------------invalid, stage {i}-------------------------------");
                foreach (string file in Directory.GetFiles($"/home/clemens/repositorys/lcc/stage_{i}/invalid"))
                {
                    Console.WriteLine("-------------");
                    List<Token> tokens = TestLexer(file, 0);
                    Node programNode = TestParser(tokens, file, 0);
                    TestGenerator(programNode, 1);
                }
                */
            }
        }
    }
}