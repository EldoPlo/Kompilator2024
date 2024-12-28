using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace Kompilator2024;

public class CodeGenerator
{
    public long GetConstToMemory(long @const, long memoryIndex, StringBuilder sb)
    {
        long offset = 1;
        sb.AppendLine($"SET {@const}");
        if (memoryIndex != 0)
        {
            sb.AppendLine($"STORE {memoryIndex}");
            offset++;
        } 

        return offset; 
    }

    public long GetVarToMemory(Variable var, long memoryIndex, StringBuilder sb)
    {
        long offset = 1;
        sb.AppendLine($"LOAD {var.Address}");
        if (memoryIndex != 0)
        {
            sb.AppendLine($"STORE {memoryIndex}");
            offset++;
        } 

        return offset;
    }

    public long GetVarVal(Variable var, long memoryindex, StringBuilder sb)
    {
        long offset = 0;
        if (!var.IsSet)
        {
            offset += GetConstToMemory((long)var.GetValue()!, memoryindex, sb);
        }
        else
        {
            offset += GetVarToMemory(var, memoryindex, sb);
        }

        return offset;
    }

    public long Write(Variable var, StringBuilder sb)
    {
        Console.WriteLine($"Generowanie WRITE: PUT {var.Address}");
        sb.AppendLine($"PUT {var.Address}");
        Console.WriteLine($"Po WRITE, CodeBuilder: {sb.ToString()}");
        return 1;
    }

   
    public long Assign(Variable var, StringBuilder sb)
    {
        Console.WriteLine($"Generowanie ASSIGN: STORE {var.Address}");
        sb.AppendLine($"STORE {var.Address}"); 
        return 1;
    }

   
    public long Read(Variable var, StringBuilder sb)
    {
        Console.WriteLine($"Generowanie READ: GET {var.Address}");
        sb.AppendLine($"GET {var.Address}");
        Console.WriteLine($"Po READ, CodeBuilder: {sb.ToString()}");
        return 1;
    }

    public long Add(Variable var1, Variable var2, StringBuilder sb)
    {
        
        sb.AppendLine($"LOAD {var1.Address}");
        sb.AppendLine($"ADD {var2.Address}");
        
        return 1;
    }
    
    public long Sub(Variable var1, Variable var2, StringBuilder sb)
    {
        
        sb.AppendLine($"LOAD {var1.Address}");
        sb.AppendLine($"SUB {var2.Address}");
        
        return 1;
    }

    public long Mul(Variable var1, Variable var2,StringBuilder sb)
    {
        // Mem
        // [0, 0, 0, 0, 0, 0] 
        
        // Load first operand to memory
        sb.AppendLine("SET 0"); // Memory[3] = result (initially 0)
        sb.AppendLine("STORE 3");

        // Inicjalizacja rejestru znaku (Memory[5])
        sb.AppendLine("SET 0"); // Memory[5] = sign (initially 0)
        sb.AppendLine("STORE 5");

        // Inicjalizacja liczb
        sb.AppendLine($"LOAD {var1.Address}"); // Załaduj var1
        sb.AppendLine("STORE 1"); // Przechowaj var1 w pamięci[1]

        sb.AppendLine($"LOAD {var2.Address}"); // Załaduj var2
        sb.AppendLine("STORE 2"); // Przechowaj var2 w pamięci[2]

        // [0, var1, var2, 0, 0, 0] 
        // Sprawdź znak pierwszej liczby (var1)
        sb.AppendLine("LOAD 1");
        sb.AppendLine("JZERO 42"); // Jeśli var1 == 0, zakończ (skocz do końca mnożenia)
        sb.AppendLine("LOAD 2");
        sb.AppendLine("JZERO 40"); // Jeśli var2 == 0, zakończ (wynik = 0)

        sb.AppendLine("LOAD 1");
        sb.AppendLine("JPOS 7"); // Jeśli var1 > 0, kontynuuj
        sb.AppendLine("SET 0");
        sb.AppendLine("SUB 1"); // Negacja var1
        sb.AppendLine("STORE 1");

        sb.AppendLine("LOAD 5");
        sb.AppendLine("ADD 1"); // Zmieniamy znak na przeciwny
        sb.AppendLine("STORE 5");

        // Sprawdź znak drugiej liczby (var2)
        sb.AppendLine("LOAD 2");
        sb.AppendLine("JPOS 7"); // Jeśli var2 > 0, kontynuuj

        // Negacja var2 (jeśli var2 < 0)
        sb.AppendLine("SET 0");
        sb.AppendLine("SUB 2");
        sb.AppendLine("STORE 2");

        sb.AppendLine("LOAD 5");
        sb.AppendLine("SUB 1"); // Zmieniamy znak na przeciwny
        sb.AppendLine("STORE 5");

        // [sign, |var1|, |var2|, 0, 0, sign] 

        // Główna pętla
        sb.AppendLine("LOAD 1"); // Załaduj var1
        // [var1, var1, var2, res, 0, sign] 

        // Sprawdzamy, czy var1 jest nieparzyste (czy var1 * 2 == var1 po HALFiE)
        sb.AppendLine("HALF");  // [fl(var1/2), var1, var2, res, 0, sign] 
        sb.AppendLine("STORE 4");// Podziel var1 przez 2 i zapisz tymczasowo w Memory[4]
        sb.AppendLine("LOAD 4"); // // [fl(var1/2), var1, var2, res, fl(var1/2), sign]
        sb.AppendLine("ADD 4");    // Pomnóż przez 2
        sb.AppendLine("SUB 1");    // Porównaj z oryginalnym var1
        sb.AppendLine("JZERO 4"); // Jeśli wynik == 0, liczba jest parzysta (skocz dalej)

        // Jeśli var1 jest nieparzysta, przechodzimy dalej
        

        // Podwój var2
        
        // [1, var1, var2, 0, fl(var1/2), sign]
        sb.AppendLine("LOAD 3");
        sb.AppendLine("ADD 2");
        sb.AppendLine("STORE 3");
        // [1, var1, var2, var2, fl(var1/2), sign]
        
        // [1|0, var1, var2, res, fl(var1/2), sign]
        sb.AppendLine("LOAD 2"); // [var2, var1, var2, res, fl(var1/2), sign]
        sb.AppendLine("ADD 2"); // [2 * var2, var1, var2, res, fl(var1/2), sign]
        sb.AppendLine("STORE 2"); // [2 * var2, var1, 2 * var2, res, fl(var1/2), sign]
        sb.AppendLine("LOAD 1"); // [var1, var1, 2 * var2, res, fl(var1/2), sign]
        sb.AppendLine("HALF"); // // [fl(var1/2), var1, 2 * var2, res, fl(var1/2), sign]
        sb.AppendLine("STORE 1");
        sb.AppendLine("JZERO 2"); //JESLI VAR1 DOSZLO DO 0
        sb.AppendLine("JUMP -17"); // Skok do sprawdzania warunku pętli
        // Zakończenie i zapisanie wyniku
        sb.AppendLine("LOAD 5"); // Załaduj rejestr znaku

        sb.AppendLine("JZERO 4"); // Jeśli znak == 0 (dodatni), pomiń negację wyniku

        // Negacja wyniku, jeśli znak == 1 (ujemny)
        sb.AppendLine("SET 0");
        sb.AppendLine("SUB 3");
        sb.AppendLine("STORE 3");
        // Wynik w p0
        sb.AppendLine("LOAD 3");
        return 1;


    }

    public long Div(Variable var1, Variable var2, StringBuilder sb)
    {
        // Mem
        // [0, 0, 0, 0, 0, 0] 
        
        // Load first operand to memory
        //Imicjalizacja 0 i 1 w odpowienio Memory[10] i Memory [11]
        sb.AppendLine("SET 0");  
        sb.AppendLine("STORE 10");
        sb.AppendLine("SET 1");
        sb.AppendLine("STORE 11");

        sb.AppendLine("LOAD 10");
        sb.AppendLine("STORE 3");
        
        //Initialize temp var to division loop
        sb.AppendLine("LOAD 10");
        sb.AppendLine("STORE 6"); //MEmory[6] = 0 
        sb.AppendLine("LOAD 10");
        sb.AppendLine("STORE 7"); //MEmory[7] = 0 

        // Inicjalizacja rejestru znaku (Memory[5])
        sb.AppendLine("LOAD 10"); // Memory[5] = sign (initially 0)
        sb.AppendLine("STORE 5");

        // Inicjalizacja liczb
        sb.AppendLine($"LOAD {var1.Address}"); // Załaduj var1
        sb.AppendLine("STORE 1"); // Przechowaj var1 w pamięci[1]

        sb.AppendLine($"LOAD {var2.Address}"); // Załaduj var2
        sb.AppendLine("STORE 2"); // Przechowaj var2 w pamięci[2]
        
        // [0, var1, var2, 0, 0, 0] 
        // Sprawdź znak pierwszej liczby (var1)
        sb.AppendLine("LOAD 1");
        sb.AppendLine("JZERO 83"); // Jeśli var1 == 0, zakończ (skocz do końca mnożenia)
        sb.AppendLine("LOAD 2");
        sb.AppendLine("JZERO 81"); // Jeśli var2 == 0, zakończ (wynik = 0)

        sb.AppendLine("LOAD 1");
        sb.AppendLine("JPOS 7"); // Jeśli var1 > 0, kontynuuj
        sb.AppendLine("LOAD 10");
        sb.AppendLine("SUB 1"); // Negacja var1
        sb.AppendLine("STORE 1");

        sb.AppendLine("LOAD 5");
        sb.AppendLine("ADD 1"); // Zmieniamy znak na przeciwny
        sb.AppendLine("STORE 5");

        // Sprawdź znak drugiej liczby (var2)
        sb.AppendLine("LOAD 2");
        sb.AppendLine("JPOS 7"); // Jeśli var2 > 0, kontynuuj

        // Negacja var2 (jeśli var2 < 0)
        sb.AppendLine("LOAD 10");
        sb.AppendLine("SUB 2");
        sb.AppendLine("STORE 2");

        sb.AppendLine("LOAD 5");
        sb.AppendLine("SUB 1"); // Zmieniamy znak na przeciwny
        sb.AppendLine("STORE 5");
        
        
        // [sign, |var1|, |var2|, 0, 0, sign,0]
        sb.AppendLine("PUT 1");
        sb.AppendLine("PUT 2");
        sb.AppendLine("LOAD 1");
        sb.AppendLine("STORE 6");
        
        //Główna Pętla
        sb.AppendLine("LOAD 7");
        sb.AppendLine("ADD 11");
        sb.AppendLine("STORE 7");
        sb.AppendLine("LOAD 6");
        sb.AppendLine("HALF");
        sb.AppendLine("STORE 6");
        sb.AppendLine("JZERO 2");
        sb.AppendLine("JUMP -7");

        sb.AppendLine("PUT 7");
        sb.AppendLine("PUT 11");
        // [0, |var1|, |var2|, 0, 0, Mem[5]=sign, MEM[6]=temp, MEM[7]=i]
        sb.AppendLine("LOAD 1");
        sb.AppendLine("STORE 6");
        
        sb.AppendLine("LOAD 7");
        sb.AppendLine("SUB 11");
        sb.AppendLine("STORE 7");
        sb.AppendLine("JNEG 37");
        //calc temp + divisor << i
        //SHIFTING B (B x 2^i)
        sb.AppendLine("LOAD 2");
        sb.AppendLine("STORE 8"); //STORE VAR2 IN MEM[8]
        sb.AppendLine("LOAD 7");
        sb.AppendLine("STORE 9"); //STORE I IN MEM[9]
        sb.AppendLine("LOAD 9");
        sb.AppendLine("JZERO 8"); // IF i <=0 skip shifting
        sb.AppendLine("LOAD 8");
        sb.AppendLine("ADD 8");   //Current Var2 * 2
        sb.AppendLine("STORE 8");
        sb.AppendLine("LOAD 9");
        sb.AppendLine("SUB 11"); //I - 1
        sb.AppendLine("STORE 9");
        sb.AppendLine("JUMP -8"); //JUMP to check if i > 0

        sb.AppendLine("LOAD 8");
        sb.AppendLine("SUB 6");

        sb.AppendLine("JPOS -19"); //IF COND
        
        //IN IF
        sb.AppendLine("LOAD 6");
        sb.AppendLine("SUB 8");
        sb.AppendLine("STORE 6");
        
        //Q += 1 << i
        sb.AppendLine("LOAD 11"); //INC ACC
        sb.AppendLine("STORE 13");
        sb.AppendLine("LOAD 7");
        sb.AppendLine("STORE 9"); //STORE I IN MEM[9]
        sb.AppendLine("LOAD 9");
        sb.AppendLine("JZERO 8"); // IF i <=0 skip shifting
        sb.AppendLine("LOAD 13");
        sb.AppendLine("ADD 13");   //Current Var2 * 2
        sb.AppendLine("STORE 13");
        sb.AppendLine("LOAD 9");
        sb.AppendLine("SUB 11"); //I - 1
        sb.AppendLine("STORE 9");
        sb.AppendLine("JUMP -8"); //JUMP to check if i > 0
        sb.AppendLine("LOAD 3");
        sb.AppendLine("ADD 13");
        sb.AppendLine("STORE 3");

        sb.AppendLine("JUMP -39");

        sb.AppendLine("PUT 3");
        
        sb.AppendLine("LOAD 5"); // Załaduj rejestr znaku

        sb.AppendLine("JZERO 5"); // Jeśli znak == 0 (dodatni), pomiń negację wyniku

        // Negacja wyniku, jeśli znak == 1 (ujemny)
        sb.AppendLine("SET 0");
        sb.AppendLine("SUB 3");
        sb.AppendLine("SUB 11");
        sb.AppendLine("STORE 3");
        // Wynik w p0
        sb.AppendLine("LOAD 3");
        return 1;

    }

    public long Mod(Variable var1, Variable var2, StringBuilder sb)
    {
        Div(var1, var2, sb); 
        // QUOTIENT IN MEM[0]

        sb.AppendLine("STORE 14");
        var quotVar = new Variable(14);
        Mul(var2, quotVar, sb);
        // RES IN MEM[0]
        
        sb.AppendLine("STORE 1");
        
        sb.AppendLine($"LOAD {var2.Address}");
        sb.AppendLine("JZERO 3");
        
        sb.AppendLine($"LOAD {var1.Address}");
        sb.AppendLine("SUB 1");

        return 1;
    }

    public long Eq(Variable var1, Variable var2, StringBuilder sb)
    {
        long offset = 0;
        sb.AppendLine($"LOAD {var1.Address}");
        sb.AppendLine("STORE 1");

        sb.AppendLine($"LOAD {var2.Address}");
        
        sb.AppendLine("SUB 1");
        sb.AppendLine("JZERO 2");
        offset += 5;
        return offset;
    }

    public long Neq(Variable var1, Variable var2, StringBuilder sb)
    {
        long offset = 0;
        sb.AppendLine($"LOAD {var1.Address}"); //Ładujemy 1 zmienna
        sb.AppendLine("STORE 1"); // STORUJEMY W REG1
        sb.AppendLine($"LOAD {var2.Address}"); // ŁADUJEMY DO ACC VAR2
        sb.AppendLine("SUB 1"); // ODEJMUJEMY OD VAR2 VAR1
        
        //JESLI JEST MNIEJSZE LUB WIĘKSZE (CYLI RÓŻNE) TO WSKAKUJEMY
        sb.AppendLine("JPOS 3");
        sb.AppendLine("JNEG 2");
        offset += 6;
        return offset;
    }
    
    public long Ge(Variable var1, Variable var2, StringBuilder sb)
    {
        long offset = 0;
        sb.AppendLine($"LOAD {var1.Address}");
        sb.AppendLine("STORE 1");

        sb.AppendLine($"LOAD {var2.Address}");
        
        sb.AppendLine("SUB 1");
        sb.AppendLine("JNEG 2");
        offset += 5;
        
        return offset;
    }
    
    
    
    public long Le(Variable var1, Variable var2, StringBuilder sb)
    {
        long offset = 0;
        sb.AppendLine($"LOAD {var1.Address}");
        sb.AppendLine("STORE 1");
    
        sb.AppendLine($"LOAD {var2.Address}");
        
        sb.AppendLine("SUB 1");
        sb.AppendLine("JPOS 2");
        offset += 5;
        return offset;
    }
    
    public long Leq(Variable var1, Variable var2, StringBuilder sb)
    {
        long offset = 0;
        sb.AppendLine($"LOAD {var1.Address}");
        sb.AppendLine("STORE 1");
    
        sb.AppendLine($"LOAD {var2.Address}");
        
        sb.AppendLine("SUB 1");
        sb.AppendLine("JPOS 3");
        sb.AppendLine("JZERO 2");
        offset += 6;
        return offset;
    }
    
    public long Geq(Variable var1, Variable var2, StringBuilder sb)
    {
        long offset = 0;
        sb.AppendLine($"LOAD {var1.Address}");
        sb.AppendLine("STORE 1");
    
        sb.AppendLine($"LOAD {var2.Address}");
        
        sb.AppendLine("SUB 1");
        sb.AppendLine("JNEG 3");
        sb.AppendLine("JZERO 2");
        offset += 6;
        return offset;
    }
    
    public long If_block(long offset, StringBuilder sb)
    {
        sb.AppendLine("JUMP" + " " + (offset+1));
        return 1;
    }
    
    public void If_else_block_first(long offset, StringBuilder sb)
    {
        sb.AppendLine("JUMP" + " " + (offset+2));
        
    }
    
    public void If_else_block_sec(long offset, StringBuilder sb)
    {
        sb.AppendLine("JUMP" + " " + (offset+1));
        
    }
    
    




    public void End(StringBuilder sb)
    {
        sb.AppendLine("HALT");
    }
    
}