using System;
using System.IO;

class Compiler
{
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Użycie: kompilator <nazwa pliku wejściowego> <nazwa pliku wyjściowego>");
            return;
        }

        string inputFileName = args[0];
        string outputFileName = args[1];

        try
        {
            
            if (!File.Exists(inputFileName))
            {
                Console.WriteLine($"Błąd: Plik wejściowy \"{inputFileName}\" nie istnieje.");
                return;
            }

           
            string inputContent = File.ReadAllText(inputFileName);
            Console.WriteLine($"Wczytano zawartość pliku wejściowego:\n{inputContent}");

            
            var compiler = new Kompilator2024.Compilation();
            Console.WriteLine("Rozpoczynanie obliczeń...");

            
            compiler.Calculate(inputContent, outputFileName);
            if (compiler.isValid)
            {
                return;
            }
            Console.WriteLine("Obliczenia zakończone pomyślnie.");

           
            Console.WriteLine($"Przetwarzanie zakończone. Wynik zapisano w pliku \"{outputFileName}\".");
        }
        catch (Exception ex)
        {
            
            Console.WriteLine($"Błąd: {ex.Message}");
            Console.WriteLine($"Szczegóły błędu: {ex.StackTrace}");
        }
    }
}