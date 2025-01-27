using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace Kompilator2024;

public class CodeGenerator
{
    private MemoryHandler _memoryHandler;
    
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

    public long CopyValue(Variable src, Variable dst, StringBuilder sb)
    {
        var offset = 0L;
        offset += GetVarVal(src, 3, sb);
        offset += SaveVarVal(dst, 3, sb);
        return offset;
    }

    public long LoadValue(Variable var, StringBuilder sb)
    {
        sb.AppendLine($"LOAD {var.Address}");
        return 1;
    }

    public long InitConstants(StringBuilder sb)
    {
        sb.AppendLine("SET 1");
        sb.AppendLine("STORE 11");
        sb.AppendLine("SET 0");
        sb.AppendLine("STORE 10");
        return 4;
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

    private long LoadArrAddress(Variable var, StringBuilder sb)
    {
        sb.AppendLine($"LOAD {var.AdressVariable.Address}");
        sb.AppendLine($"SUB {var.ArrayOffsetVariable.Address}");
        sb.AppendLine($"ADD {var.ArrayAddressVariable.Address}");
        return 3;
    }

  
    public long GetArrVarToMem(Variable var, long memoryIndex, StringBuilder sb)
    {
        
        // sb.AppendLine($"ADD {var.ArrayOffset}");
        var offset = 0L;
        offset += LoadArrAddress(var, sb);
        sb.AppendLine($"LOADI 0");
        sb.AppendLine($"STORE {memoryIndex}");
        

        return offset + 2;
    }
    //1 2 3 4 5 6 7 8 9 10
    //[][][][][][][][][][]
    
    public long GetVarVal(Variable var, long memoryindex, StringBuilder sb)
    {
        long offset = 0;
        if (var.IsArray)
        {
            offset += GetArrVarToMem(var, memoryindex, sb);
        }
        
        else
        {
            offset += GetVarToMemory(var, memoryindex, sb);
        }

        return offset;
    }

    public long SaveArrToVarMem(long memoryIndex, Variable destArrVar, StringBuilder sb)
    {
        
        sb.AppendLine($"LOAD {memoryIndex}");
        sb.AppendLine("STORE 2");
        var offset = LoadArrAddress(destArrVar, sb);
        sb.AppendLine($"STORE 1");
        sb.AppendLine($"LOAD 2");
        sb.AppendLine($"STOREI 1");
        return 5 + offset;
    }
    
    public long SaveToVarMem(long memoryIndex, Variable destVar, StringBuilder sb)
    {
        sb.AppendLine($"LOAD {memoryIndex}");
        sb.AppendLine($"STORE {destVar.Address}");
        return 2;
    }

    public long SaveVarVal(Variable var, long memoryindex, StringBuilder sb)
    {
        if (var == null)
        {
            throw new ArgumentNullException(nameof(var), "Variable cannot be null");
        }

        long offset = 0;
        if (var.IsArray)
        {
            // Sprawdzamy, czy zmienna tablicowa jest prawidłowo ustawiona
            if (var.ArrayAddressVariable == null)
            {
                throw new InvalidOperationException($"Array address variable for {var.Name} is not initialized.");
            }
            offset += SaveArrToVarMem(memoryindex, var, sb);
        }
        else
        {
            
            if (var.Address == 0 && var.Name != "validsymbol")
            {
                throw new InvalidOperationException($"Address for {var.Name} is not initialized.");
            }
            offset += SaveToVarMem(memoryindex, var, sb);
        }

        return offset;
    }

    public long Write(Variable var, StringBuilder sb)
    {
        var offset = 0L;
        offset += GetVarVal(var, 0, sb);
        sb.AppendLine($"PUT 0");
        return 1 + offset;
    }

   
    public long Assign(Variable destVar, Variable srcVar, StringBuilder sb)
    {

        long offset = 0L;
        offset += GetVarVal(srcVar, 0, sb);
        offset += SaveVarVal(destVar, 0, sb);
        return offset;
    }

   
    public long Read(Variable var, StringBuilder sb)
    {
        sb.AppendLine($"GET {var.Address}");
        return 1;
    }

    public long Add(Variable var1, Variable var2, StringBuilder sb)
    {
        long offset = 0;
        offset+= GetVarVal(var1, 3, sb);
        offset+= GetVarVal(var2, 4, sb);
        sb.AppendLine($"LOAD 3");
        sb.AppendLine($"ADD 4");
        
        return offset + 2;
    }
    
    public long Sub(Variable var1, Variable var2, StringBuilder sb)
    {
        long offset = 0;
        offset+= GetVarVal(var1, 3, sb);
        offset+= GetVarVal(var2, 4, sb);
        sb.AppendLine("LOAD 3");
        sb.AppendLine("SUB 4");
        
        return offset + 2;
    }

    public long Mul(Variable var1, Variable var2,StringBuilder sb)
    {
        // Mem
        // [0, 0, 0, 0, 0, 0] 
        long offset_start = sb.ToString().Trim().Split("\n", StringSplitOptions.RemoveEmptyEntries).LongLength;
        GetVarVal(var1, 3, sb);
        GetVarVal(var2, 4, sb);
        // Inicjalizacja liczb
        sb.AppendLine($"LOAD 3"); // Załaduj var1
        sb.AppendLine("STORE 1"); // Przechowaj var1 w pamięci[1]

        sb.AppendLine($"LOAD 4"); // Załaduj var2
        sb.AppendLine("STORE 2"); // Przechowaj var2 w pamięci[2]
        
        // Load first operand to memory
        sb.AppendLine("LOAD 10"); // Memory[3] = result (initially 0)
        sb.AppendLine("STORE 3");
        sb.AppendLine("STORE 4");
        // Inicjalizacja rejestru znaku (Memory[5])
        sb.AppendLine("STORE 5");



        // [0, var1, var2, 0, 0, 0] 
        // Sprawdź znak pierwszej liczby (var1)
        sb.AppendLine("LOAD 1");
        sb.AppendLine("JZERO 42"); // Jeśli var1 == 0, zakończ (skocz do końca mnożenia)
        sb.AppendLine("LOAD 2");
        sb.AppendLine("JZERO 40"); // Jeśli var2 == 0, zakończ (wynik = 0)

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
        sb.AppendLine("LOAD 10");
        sb.AppendLine("SUB 3");
        sb.AppendLine("STORE 3");
        // Wynik w p0
        sb.AppendLine("LOAD 3");
        long offset_end = sb.ToString().Trim().Split("\n", StringSplitOptions.RemoveEmptyEntries).LongLength;

        
        return offset_end - offset_start;
    }

    public long Div(Variable var1, Variable var2, StringBuilder sb)
    {
        // Mem
        // [0, 0, 0, 0, 0, 0] 
        
        // Load first operand to memory
        //Imicjalizacja 0 i 1 w odpowienio Memory[10] i Memory [11]
        ;
        long offset_start = sb.ToString().Trim().Split("\n", StringSplitOptions.RemoveEmptyEntries).LongLength;
        GetVarVal(var1, 3, sb);
        GetVarVal(var2, 4, sb);

        sb.AppendLine("LOAD 3"); // Załaduj var1
        sb.AppendLine("STORE 1"); // Przechowaj var1 w pamięci[1]

        sb.AppendLine("LOAD 4"); // Załaduj var2
        sb.AppendLine("STORE 2"); // Przechowaj var2 w pamięci[2]
        
        sb.AppendLine("LOAD 10");
        sb.AppendLine("STORE 3");
        sb.AppendLine("STORE 4");
        
        //Initialize temp var to division loop

        sb.AppendLine("STORE 5");

        // Inicjalizacja liczb
        
        
        // [0, var1, var2, 0, 0, 0] 
        // Sprawdź znak pierwszej liczby (var1)
        sb.AppendLine("LOAD 1");
        sb.AppendLine("JZERO 78"); // Jeśli var1 == 0, zakończ (skocz do końca mnożenia)
        sb.AppendLine("LOAD 2");
        sb.AppendLine("JZERO 76"); // Jeśli var2 == 0, zakończ (wynik = 0)

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

        
        sb.AppendLine("LOAD 5"); // Załaduj rejestr znaku

        sb.AppendLine("JZERO 5"); // Jeśli znak == 0 (dodatni), pomiń negację wyniku

        // Negacja wyniku, jeśli znak == 1 (ujemny)
        sb.AppendLine("SET 0");
        sb.AppendLine("SUB 3");
        sb.AppendLine("SUB 11");
        sb.AppendLine("STORE 3");
        // Wynik w p0
        sb.AppendLine("LOAD 3");
        long offset_end = sb.ToString().Trim().Split("\n", StringSplitOptions.RemoveEmptyEntries).LongLength;

        return offset_end - offset_start;

    }

    public long Mod(Variable var1, Variable var2, StringBuilder sb)
    {
        long offset_start = sb.ToString().Trim().Split("\n", StringSplitOptions.RemoveEmptyEntries).LongLength;
        
        Div(var1, var2, sb); 
        // QUOTIENT IN MEM[0]

        sb.AppendLine("STORE 14");
        var quotVar = new Variable(14);
        Mul(var2, quotVar, sb);
        // RES IN MEM[0]
        
        sb.AppendLine("STORE 1");
        
        long offset = 0;
        offset+= GetVarVal(var1, 3, sb);
        offset+= GetVarVal(var2, 4, sb);

        sb.AppendLine("LOAD 3"); // Załaduj var1
        sb.AppendLine("STORE 5"); // Przechowaj var1 w pamięci[1]

        sb.AppendLine("LOAD 4"); // Załaduj var2
        sb.AppendLine("STORE 2"); // Przechowaj var2 w pamięci[2]
        
        
        sb.AppendLine($"LOAD 2");
        sb.AppendLine("JZERO 3");
        
        sb.AppendLine($"LOAD 5");
        sb.AppendLine("SUB 1");

        long offset_end = sb.ToString().Trim().Split("\n", StringSplitOptions.RemoveEmptyEntries).LongLength;

        return offset_end - offset_start;
    }

    public long Eq(Variable var1, Variable var2, StringBuilder sb)
    {
        long offset = 0;
        offset+= GetVarVal(var1, 3, sb);
        offset+= GetVarVal(var2, 4, sb);

        sb.AppendLine("LOAD 3"); // Załaduj var1

        sb.AppendLine("SUB 4");
        sb.AppendLine("JZERO 2");
        offset += 3;
        return offset;
    }

    public long Neq(Variable var1, Variable var2, StringBuilder sb)
    {
        long offset = 0;
        offset+= GetVarVal(var1, 3, sb);
        offset+= GetVarVal(var2, 4, sb);

        sb.AppendLine("LOAD 3"); // Załaduj var1

        sb.AppendLine("SUB 4");
        
        //JESLI JEST MNIEJSZE LUB WIĘKSZE (CYLI RÓŻNE) TO WSKAKUJEMY
        sb.AppendLine("JPOS 3");
        sb.AppendLine("JNEG 2");
        offset += 4;
        return offset;
    }
    
    public long Ge(Variable var1, Variable var2, StringBuilder sb)
    {
        long offset = 0;
        offset+= GetVarVal(var1, 3, sb);
        offset+= GetVarVal(var2, 4, sb);

        sb.AppendLine("LOAD 4"); // Załaduj var1

        sb.AppendLine("SUB 3");
        sb.AppendLine("JNEG 2");
        offset += 3;
        
        return offset;
    }
    
    
    
    public long Le(Variable var1, Variable var2, StringBuilder sb)
    {
        long offset = 0;
        offset+= GetVarVal(var1, 3, sb);
        offset+= GetVarVal(var2, 4, sb);

        sb.AppendLine("LOAD 4"); // Załaduj var1

        sb.AppendLine("SUB 3");
        sb.AppendLine("JPOS 2");
        offset += 3;
        return offset;
    }
    
    public long Leq(Variable var1, Variable var2, StringBuilder sb)
    {
        long offset = 0;
        offset+= GetVarVal(var1, 3, sb);
        offset+= GetVarVal(var2, 4, sb);

        sb.AppendLine("LOAD 4"); // Załaduj var1

        sb.AppendLine("SUB 3");
        sb.AppendLine("JPOS 3");
        sb.AppendLine("JZERO 2");
        offset += 4;
        return offset;
    }
    
    public long Geq(Variable var1, Variable var2, StringBuilder sb)
    {
        long offset = 0;
        offset+= GetVarVal(var1, 3, sb);
        offset+= GetVarVal(var2, 4, sb);

        sb.AppendLine("LOAD 4"); // Załaduj var1

        sb.AppendLine("SUB 3");;
        sb.AppendLine("JNEG 3");
        sb.AppendLine("JZERO 2");
        offset += 4;
        return offset;
    }
    
    public long If_block(long offset, StringBuilder sb)
    {
        sb.AppendLine("JUMP" + " " + (offset+1));
        return 1;
    }
    
    public long If_else_block_first(long offset, StringBuilder sb)
    {
        sb.AppendLine("JUMP" + " " + (offset+2));
        return 1;
    }
    
    public long If_else_block_sec(long offset, StringBuilder sb)
    {
        sb.AppendLine("JUMP" + " " + (offset+1));
        return 1;
    }

    public long While_first_block(long offset, StringBuilder sb)
    {
        sb.AppendLine($"JUMP {offset + 2}");
        return 1;
    }

    public long While_sec_block(long commandoffset,long condoffset,StringBuilder sb)
    {
        sb.AppendLine($"JUMP -{commandoffset + condoffset + 1}");
        return 1;
    }


    public long Repeat_block(long commandoffset, long condoffset, StringBuilder sb)
    {
        sb.AppendLine($"JUMP -{commandoffset + condoffset}");
        return 1;
    }

    public long For_block_init(ForLabel label, Variable start, Variable end, StringBuilder sb)
    {
        var offset = 0L;
        offset += CopyValue(start, label.Iterator, sb);
        offset += CopyValue(end, label.End, sb);

        return offset;
    }
    
    public long For_to_block_first(ForLabel label, StringBuilder sb)
    {
        return Leq(label.Iterator, label.End, sb);
    }

    public long For_downto_block_first(ForLabel label, StringBuilder sb)
    {
        return Geq(label.Iterator, label.End, sb);
    }
    
    public long For_to_block_second(ForLabel label, long offsetCond, StringBuilder sb)
    {
        sb.AppendLine($"LOAD {label.Iterator.Address}");
        sb.AppendLine($"ADD 11");
        sb.AppendLine($"STORE {label.Iterator.Address}");
        sb.AppendLine($"JUMP -{offsetCond + GetLineCount(sb) + 1}");
        return 4;
    }
    
    public long For_downto_block_second(ForLabel label, long offsetCond, StringBuilder sb)
    {
        sb.AppendLine($"LOAD {label.Iterator.Address}");
        sb.AppendLine($"SUB 11");
        sb.AppendLine($"STORE {label.Iterator.Address}");
        sb.AppendLine($"JUMP -{offsetCond + GetLineCount(sb) + 1}");
        return 4;
    }
    public long For_block_addJump(long offsetCommands, StringBuilder sb)
    {
        sb.AppendLine("JUMP " + (offsetCommands+1));
        return 1;
    }
    
    long GetConstant(Variable var, StringBuilder sb)
    {
        return GetVarVal(var,1, sb);
    }
    
    public long GetLineCount(StringBuilder sb)
    {
        if (sb == null || sb.Length == 0)
            return 0;
        
        return sb.ToString().Count(c => c == '\n');
    }
    

    public long End(StringBuilder sb)
    {
        sb.AppendLine("HALT");
        return 1;
    }
    
}