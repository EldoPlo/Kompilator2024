using System;
using System.IO;

class Compiler
{
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Use: compiler <nazwa pliku wejściowego> <nazwa pliku wyjściowego>");
            return;
        }

        string inputFileName = args[0];
        string outputFileName = args[1];

        try
        {
            
            if (!File.Exists(inputFileName))
            {
                Console.WriteLine($"Error: File input \"{inputFileName}\" doesn't exist.");
                return;
            }

           
            string inputContent = File.ReadAllText(inputFileName);
            Console.WriteLine($"Loaded content of the file input:\n{inputContent}");

            
            var compiler = new Kompilator2024.Compilation();
            Console.WriteLine("Start Processing...");

            
            compiler.Calculate(inputContent, outputFileName);
            if (compiler.isValid)
            {
                return;
            }

            Console.WriteLine($"Processing ended. Result saved in file :  \"{outputFileName}\".");
        }
        catch (Exception ex)
        {
            
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Error Details: {ex.StackTrace}");
        }
    }
}