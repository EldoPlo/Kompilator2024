namespace Kompilator2024;

public class Symbol
{
    public string Name;
    public long Offset;
    public long Value;

    
     private bool isInitialized;
     private  bool isArray;
     private  bool isIterator;

    
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
        Size = 1;
        ArrayBeginIdx = 0;
        ArrayEndIdx = 0;
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