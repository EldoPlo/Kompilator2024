using Antlr4.Runtime.Misc;

namespace Kompilator2024;

public class MemoryHandler
{
    private Dictionary<string, Symbol> _memSymbolMap = new Dictionary<string, Symbol>();
    private long _memoryEndPointer = 15;
    private int _tempVariableCounter = 0;
    private bool Errorfound;
    private long ErrorCounter;
    private  String ErrorTextColor = "\\u001B31;1m";
    private long currColumn;
    private long currLine;
    public bool CheckIfSymbolExists(string name)
    {
        return _memSymbolMap.ContainsKey(name);
    }

    public long GetSymbol(string symbol)
    {
        if (CheckIfSymbolExists(symbol))
        {
            Console.WriteLine($"Symbol '{symbol}' is already defined.");
            
            return _memSymbolMap[symbol].Offset;
        }

        long offset = AllocMemorySingle();
        _memSymbolMap.Add(symbol, new Symbol(symbol, offset));
        return offset;
    }

    public void InitConstantVariables()
    {
        _memSymbolMap.Add("1", new Symbol("1", 11));
        _memSymbolMap.Add("0", new Symbol("0", 10));
    }
    
    public Variable GetConstVariable(long value)
    {
        var name = $"{value}";

        if(CheckIfSymbolExists(name))
        {
            return new Variable(name, _memSymbolMap[name].Offset, value);
        }

        var offset = AllocMemorySingle();
        var sym = new Symbol(name, offset);
        _memSymbolMap.Add(name,sym);
        return new Variable(name, offset, value);
    }
    private long AllocMemorySingle()
    {
        _memoryEndPointer++;
        return _memoryEndPointer-1;
    }

    public void SetLocation(long line, long colum)
    {
        currColumn = colum;
        currLine = line;
    }

    public Variable GetVariable(string name)
    {
        if (_memSymbolMap.TryGetValue(name, out var symbol))
        {
            return new Variable(name, symbol.Offset);
        }

        GetSymbol(name);

        return new Variable(name, _memSymbolMap[name].Offset);
    }
    public Variable GetIterator(string name)
    {
        if(CheckIfSymbolExists(name))
        {
            if (_memSymbolMap[name].IsIterator())
            {
                return new Variable(name, _memSymbolMap[name].GotOffset());
            }
            
            Console.WriteLine(ErrorTextColor + " " + name + " already defined non-iterator with this name" + currLine + ":" + currColumn + ".");
            Errorfound = true;
            ErrorCounter++;
            return new Variable(-1);
        }

        long offset = AllocMemorySingle();
        var sym = new Symbol(name, offset, true, true);
        _memSymbolMap.Add(name, sym);
        return new Variable(name, offset);
    }

    
    public Variable CreateTemporaryVariable()
    {
        string tempName = $"temp{_tempVariableCounter++}";
        long address = GetSymbol(tempName);
        return new Variable(tempName, address);
    }
    
    public ForLabel CreateForLabel(Variable iterator, Variable start, Variable end)
    {
        return new ForLabel(iterator, start, end);
    }
    
    private long AllocMemoryArr(long size)
    {
        long beginPointer = _memoryEndPointer;
        _memoryEndPointer+=size;
        return beginPointer;
    }
    public long AddTable(string name, long begin, long end)
    {
        if (CheckIfSymbolExists(name))
        {
            Console.WriteLine($"{ErrorTextColor}Table '{name}' is already defined at {currLine}:{currColumn}.\u001B[0m");
            Errorfound = true;
            ErrorCounter++;
            return -1;
        }

        if (end < begin)
        {
            Console.WriteLine($"{ErrorTextColor}Error '{name}' endIndex is smaller than beginIndex.\u001B[0m");
            Errorfound = true;
            ErrorCounter++;
            return -1;
        }

        long offset = AllocMemoryArr(end - begin + 1);
        _memSymbolMap.Add(name, new Symbol(name, offset,begin,end));
        return offset;
    }
    
    public Variable getVariable(String name)
    {
        Symbol sym = _memSymbolMap[name];

        if(sym.isArrayy())
        {
           Console.WriteLine(ErrorTextColor + " " + name + " cant use array as variable" + currLine + ":" + currColumn + ".");
            Errorfound = true;
            ErrorCounter++;
            return null;
        }

        return new Variable(sym.GotOffset());
    }
    
    public Variable GetArrValNum(String name, long pos)
    {
        Symbol sym = _memSymbolMap[name];

        if(sym == null)
        {
            return null;
        }

        if(!sym.isArrayy())
        {
            Console.WriteLine($"{ErrorTextColor}Error '{name}' variable is not in array '{currLine} : '{currColumn}'.\u001B[0m");
            Errorfound = true;
            ErrorCounter++;
            return null;
        }

        if(sym.ArrayEndIdx < pos || sym.ArrayBeginIdx > pos )
        {
            Console.WriteLine($"{ErrorTextColor}Error '{name}'    ['{pos}'] out of ponds in   '{currLine}  :  '{currColumn} \u001B[0m");
            Errorfound = true;
            ErrorCounter++;
            return null;
        }

        Variable ret_pos = GetConstVariable(pos);
        Variable offsetVar = GetConstVariable(sym.ArrayBeginIdx);
        Variable adressVar = GetConstVariable(sym.GotOffset());

        Variable ret = new Variable(sym.GotOffset(), ret_pos, offsetVar, adressVar);
        return ret;
    }
    
    public Variable GetArrValVar(String name, String pos_var)
    {
        Symbol sym = _memSymbolMap[name];
        Symbol pos = _memSymbolMap[pos_var];
        if(sym == null || pos == null)
        {
            return null;
        }
        if(!sym.isArrayy())
        {
            Console.WriteLine($"{ErrorTextColor}Error '{name}' variable is not in array '{currLine} : '{currColumn}'.\u001B[0m");
            Errorfound = true;
            ErrorCounter++;
            return null;
        }
        else if(pos.isArrayy())
        {
            Console.WriteLine($"{ErrorTextColor}Error '{name}'  position symbol is in array  '{currLine}  :  '{currColumn} \u001B[0m");
            Errorfound = true;
            ErrorCounter++;
            return null;
        }

        Variable posVar = GetVariable(pos.Name);
        Variable offsetVar = GetConstVariable(sym.ArrayBeginIdx);
        Variable adressVar = GetConstVariable(sym.GotOffset());

        Variable ret = new Variable(sym.GotOffset(), posVar, offsetVar, adressVar);
        return ret;
    }


    public Variable CopyVariable(Variable var)
    {
        var name = $"{var.Name}-Copy";
        if (CheckIfSymbolExists(name))
        {
            name += "-Copy";
        }
        var offset = AllocMemorySingle();
        _memSymbolMap.Add(name, new Symbol(name, offset, true, true));
        return new Variable(name, offset);
    }

    public void RemoveVariable(Variable var)
    {
        if (!CheckIfSymbolExists(var.Name))
        {
            return;
        }

        _memSymbolMap.Remove(var.Name);
    }
    
    


    // public Variable InitVariable(Variable var, String name)
    // {
    //     if (CheckIfSymbolExists(name))
    //     {
    //         Symbol sym = _memSymbolMap[name];
    //        sym.SetInitialized(true);
    //     }
    //
    //     if (var == null)
    //     {
    //         createNonExistingSymbolPlaceHolder();
    //     }
    //
    //     return var;
    // }

    // public Variable createNonExistingSymbolPlaceHolder()
    // {
    //     return new Variable();
    // }
    //
    // public Variable GetValue(int a)
    // {
    //     return new Variable(-1, Add(value), value);
    // }
}