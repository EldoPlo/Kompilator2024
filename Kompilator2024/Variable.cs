namespace Kompilator2024;

public class Variable
{
    public bool IsSet;
    public  long Address;
    public string Name;
    public long? Value;
    public bool IsConst = false;
    public Variable(string name, long address)
    {
        Address = address; 
        IsSet = true;
        Name = name;
        //arrayAddress = -1;
    }
    
    public Variable(long address)
    {
        Address = address;
        IsSet = true;
    }

    // Const constructor
    public Variable(string name, long address, long value)
    {
        Name = name;
        Address = address;
        Value = value;
        IsConst = true;
    }

    public long? GetValue()
    {
        return Value;
    }
    
    
}