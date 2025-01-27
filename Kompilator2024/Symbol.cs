namespace Kompilator2024;

public class Symbol
{
    public string Name;
    public long Offset;
    public long Value;
    public List<string> Parameters;
    
    
     private bool isInitialized;
     public  bool isArray;
     public  bool isIterator;
     private bool isProcedure;
     public bool isMutable;
     public bool isAsigned;
    
     private  long Size;
     public  long ArrayBeginIdx { get; set; }
     public  long ArrayEndIdx { get; set; }

    public Symbol(String name, long offset)
    {
        Name = name;
        Offset = offset;

        isInitialized = false;
        isArray = false;
        isIterator = false;
        
        Size = 1;
        ArrayEndIdx = 0;
        ArrayBeginIdx = 0;
    }

    public Symbol(String name, long offset, long arrayBeginIdx, long arrayEndIdx)
    {
        Name = name;
        Offset = offset;
    
        isInitialized = true;
        isArray = true;
        isIterator = false;
        isProcedure = false;
        
        Size = arrayEndIdx - arrayBeginIdx + 1;
        ArrayBeginIdx = arrayBeginIdx;
        ArrayEndIdx = arrayEndIdx;
       
    }

    public Symbol(String name, long offset, bool iterator, bool init)
    {
        Name= name;
        Offset = offset;
    
        isInitialized = true;
        isArray = false;
        isIterator = true;
        isProcedure = false;
        
        
        Size = 1;
        ArrayBeginIdx = 0;
        ArrayEndIdx = 0;
    }
    
    // Konstruktor dla procedur
    public Symbol(string name, long offset, List<string> parameters)
    {
        Name = name;
        Offset = offset;
        Parameters = parameters;
        isProcedure = true;
    }

    public Symbol()
    {
        Name = "validSymbol";
    }
    
    public bool isArrayy()
    {
        return isArray;
    }

    public bool IsIterator()
    {
        return isIterator;
    }

    public long GotOffset()
    {
        return Offset;
    }
}