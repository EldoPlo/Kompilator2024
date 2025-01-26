using System;
using System.IO;
using Antlr4.Runtime;

namespace Kompilator2024
{
    public class MyErrorListener : BaseErrorListener
    {
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine,
            string msg, RecognitionException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
          
            // Print the error message
            Console.WriteLine($"Syntax error at line lalala {line}:{charPositionInLine} - {msg}");

            // Reset the text color
            Console.ResetColor();        }
    }
    public class Compilation
    {
        public void Calculate(string input, string output)
        {
            var lexer = new l4Lexer(new AntlrInputStream(input));
            var tokens = new CommonTokenStream(lexer);
            var memory = new MemoryHandler();
            var codegen = new CodeGenerator();
            var parser = new l4Parser(tokens);
            var visitor = new LanguageVisitor(memory, codegen);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new MyErrorListener());
            var tree = parser.program_all();
            if (visitor.GetErrors().Count != 0 || memory.GetErrors().Count != 0)
            {
                visitor.PrintErrors();
                memory.GetErrors();
                throw new Exception("COMPILATION ERROR");
            }
            if (parser.NumberOfSyntaxErrors != 0)
            {
                throw new Exception("PROBLEM W KOMPILACJI");
            }

           
          
            try
            {
                var result = visitor.Visit(tree);
                if (result == null)
                {
                    Console.WriteLine("Błąd: Wynik odwiedzenia drzewa jest pusty (null).");
                    return;
                }

                WriteCode(output, result);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
                
            }
            

         
        }

        private void WriteCode(string path, VisitorDataTransmiter result)
        {
            Console.WriteLine($"Zawartość CodeBuilder przed zapisaniem: {result.CodeBuilder.ToString()}");
            if (result.CodeBuilder.Length == 0)
            {
                Console.WriteLine("Błąd: CodeBuilder jest pusty.");
            }
            else
            {
                // Console.WriteLine("Zawartość CodeBuilder:");
                // Console.WriteLine(result.CodeBuilder.ToString()); 
                // File.WriteAllText(path, result.CodeBuilder.ToString());
            }
        }
    }
}
