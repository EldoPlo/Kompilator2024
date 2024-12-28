namespace Kompilator2024;

public class Symbol
{
    public string Name;
    public long Offset;
    public long Value;

    
    // private bool isInitialized;
    // private  bool isArray;
    // private  bool isIterator;

    
    // private  long size;
    // private  long arrayBeginIdx;
    // private  long arrayEndIdx;

    public Symbol(String name, long offset)
    {
        Name = name;
        Offset = offset;

        // isInitialized = false;
        // isArray = false;
        // isIterator = false;
        //
        // size = 1;
        // arrayBeginIdx = 0;
        // arrayEndIdx = 0;
    }

    // public Symbol(String name, long offset, long arrayBeginIdx, long arrayEndIdx)
    // {
    //     this.name = name;
    //     this.offset = offset;
    //
    //     isInitialized = true;
    //     isArray = true;
    //     isIterator = false;
    //
    //     size = arrayEndIdx - arrayBeginIdx + 1;
    //     this.arrayBeginIdx = arrayBeginIdx;
    //     this.arrayEndIdx = arrayEndIdx;
    // }

    // public Symbol(String name, long offset, bool iterator, bool init)
    // {
    //     this.name = name;
    //     this.offset = offset;
    //
    //     isInitialized = true;
    //     isArray = false;
    //     isIterator = true;
    //     size = 1;
    //     arrayBeginIdx = 0;
    //     arrayEndIdx = 0;
    // }

    public long GotOffset()
    {
        return Offset;
    }
}