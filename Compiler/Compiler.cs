using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Compiler.Lexer;
using Compiler.Parser.Nodes;

namespace Compiler
{
    public static class Compiler
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: Compiler <input path> [optional: -v]");
            }
            else
            {
                if (args[0] != "--dev-mode--")
                {
                    NonDevMode(args);
                }
                else
                {
                    DevMode();
                }
            }
        }

        static void DevMode()
        {
            for (int i = 1; i <= 2; i++)
            {
                Console.WriteLine($"---------------------valid, stage {i}-------------------------------");
                foreach (string file in Directory.GetFiles($"/home/clemens/repositorys/lcc/stage_{i}/valid"))
                {
                    Console.WriteLine("-------------");
                    List<Token> tokens = TestLexer(file, 0);
                    TestParser(tokens, file, 1);
                }


                Console.WriteLine($"---------------------invalid, stage {i}-------------------------------");
                foreach (string file in Directory.GetFiles($"/home/clemens/repositorys/lcc/stage_{i}/invalid"))
                {
                    Console.WriteLine("-------------");
                    List<Token> tokens = TestLexer(file, 0);
                    TestParser(tokens, file, 1);
                }
            }
        }

        static void NonDevMode(string[] args)
        {
            bool debug = false;
            string inputFileName = args[0].Split("/").Last();
            string outputPath = args[0].Substring(0, args[0].LastIndexOf("/"));

            if (args.Length == 2)
            {
                if (args[2] == "-v")
                {
                    debug = true;
                }
            }

            Compile(args[0], $"{outputPath}/assembly.s", debug);
            if (debug)
            {
                Console.WriteLine($"Compiled to {outputPath}/assembly.s");
            }

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "gcc",
                Arguments = $"{outputPath}/assembly.s -o {outputPath}/{inputFileName.Replace(".c", "")}"
            };
            Process proc = new Process() {StartInfo = startInfo,};
            proc.Start();

            while (!proc.HasExited)
            {
                Thread.Sleep(1);
            }

            File.Delete($"{outputPath}/assembly.s");

            if (debug)
            {
                Console.WriteLine($"Assembled to {outputPath}/program");
                Console.WriteLine("Deleted assembly.s file. Done!");
            }
        }


        static void Compile(string inputPath, string outputPath, bool debug)
        {
            //Lexing
            Lexer.Lexer lexer = new Lexer.Lexer();


            StreamReader file = new StreamReader(inputPath);
            string contents = file.ReadToEnd();

            List<Token> tokens = lexer.Lex(contents);
            if (debug)
            {
                Console.WriteLine($"Lexed {inputPath.Split("/").Last()}.");
            }

            //Parsing
            Parser.Parser p = new Parser.Parser(tokens);

            try
            {
                Node programNode = p.Parse(NodeType.ProgramNode);
                if (debug)
                {
                    Console.WriteLine($"Parsed \"{inputPath.Split("/").Last()}\"");
                }

                //Generating
                Generator.Generator generator = new Generator.Generator();
                string program = generator.Generate(programNode);

                File.WriteAllText(outputPath, program);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in file \"{inputPath.Split("/").Last()}\"");
                Console.WriteLine(e.Message);
            }
        }

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

        static void TestGenerator(Node root, string destinationPath, int debugLevel)
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
    }
}