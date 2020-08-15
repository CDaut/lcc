using System;
using System.Collections.Generic;
using System.IO;

namespace Compiler
{
    public class Compiler
    {
        public static void Main()
        {
            TestCompiler("../../../../tests/week_1/valid");
            TestCompiler("../../../../tests/week_1/invalid");
        }

        static void TestCompiler(String path)
        {
            String[] files = Directory.GetFiles(path);
            Lexer lexer = new Lexer();

            foreach (String filename in files)
            {
                StreamReader file = new StreamReader(filename);
                String contents = file.ReadToEnd();
                
                List<Token> tokens = lexer.Lex(contents);
                
                Console.WriteLine("-----------" + filename + "-----------");
                foreach (Token token in tokens)
                {
                    Console.WriteLine(token.ToString());
                }
                Console.WriteLine("--------------------------------------");
            }
        }
    }
}