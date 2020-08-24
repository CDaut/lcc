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
                    DevFunctions.DevMode();
                }
            }
        }

        static void NonDevMode(string[] args)
        {
            bool debug = false;
            string inputFileName = args[0].Split("/").Last();
            string outputPath = args[0].Substring(0, args[0].LastIndexOf("/", StringComparison.Ordinal));

            if (args.Length == 2)
            {
                if (args[1] == "-v")
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
    }
}