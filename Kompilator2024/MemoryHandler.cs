using Antlr4.Runtime.Misc;

namespace Kompilator2024;

public class MemoryHandler
{
    private Dictionary<string, Symbol> _memSymbolMap = new Dictionary<string, Symbol>();
    private long _memoryEndPointer = 15;
    private int _tempVariableCounter = 0;

    private int currColumn;
    private int currLine;
    public bool CheckIfSymbolExists(string name)
    {
        return _memSymbolMap.ContainsKey(name);
    }

    public long AddSymbol(string symbol)
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

    public void SetLocation(int line, int colum)
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

        AddSymbol(name);

        return new Variable(name, _memSymbolMap[name].Offset);
    }
    
    public Variable CreateTemporaryVariable()
    {
        string tempName = $"temp{_tempVariableCounter++}";
        long address = AddSymbol(tempName);
        return new Variable(tempName, address);
    }
    //
    // public Variable GetValue(int a)
    // {
    //     return new Variable(-1, Add(value), value);
    // }
}