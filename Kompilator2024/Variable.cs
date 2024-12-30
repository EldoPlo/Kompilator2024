namespace Kompilator2024;

public class Variable
{
    public bool IsSet;
    public  long Address;
    public string Name;
    public long? Value;
    public bool IsConst = false;
    public Variable? ArrayOffsetVariable = null;
    public Variable? ArrayAddressVariable = null;
    public Variable? AdressVariable = null;
    public long ArrayAddress;
    public bool IsArray;
    public Variable(string name, long address)
    {
        Address = address; 
        IsSet = true;
        Name = name;
        ArrayAddress = -1;
    }
    
    public Variable(long address, long arrayAddress)
    {
        Address= address;
        ArrayAddress= arrayAddress;
        IsSet = true;
    }

    public Variable(long address, Variable addressVar, Variable arrayOffset, Variable adressVariable)
    {
        Address = address;
        ArrayAddressVariable = addressVar;
        ArrayOffsetVariable = arrayOffset;
        AdressVariable = adressVariable;
        IsSet = true;
        IsArray = true;
    }
    
    // public Variable(long address, long arrayAddress, long arrOffset)
    // {
    //     Address = address;
    //     ArrayAddress = arrayAddress;
    //     ArrayOffset = arrOffset;
    //     IsSet = true;
    //     IsArray = true;
    // }

    
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