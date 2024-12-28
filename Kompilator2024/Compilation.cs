using System;
using System.IO;
using Antlr4.Runtime;

namespace Kompilator2024
{
    public class Compilation
    {
        public void Calculate(string input, string output)
        {
            var lexer = new l4Lexer(new AntlrInputStream(input));
            var tokens = new CommonTokenStream(lexer);
            var memory = new MemoryHandler();
            var codegen = new CodeGenerator();
            var parser = new l4Parser(tokens);
            var tree = parser.program_all();

            var visitor = new LanguageVisitor(memory, codegen);
            var result = visitor.Visit(tree);

            if (result == null)
            {
                Console.WriteLine("Błąd: Wynik odwiedzenia drzewa jest pusty (null).");
                return;
            }

            WriteCode(output, result);
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
                Console.WriteLine("Zawartość CodeBuilder:");
                Console.WriteLine(result.CodeBuilder.ToString()); 
                File.WriteAllText(path, result.CodeBuilder.ToString());
            }
        }
    }
}
