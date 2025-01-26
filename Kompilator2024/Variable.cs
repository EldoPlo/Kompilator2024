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
    public bool IsParameter = false;
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

    public Variable(long address, Variable addressVar, Variable arrayOffset, Variable adressVariable, string name = "")
    {
        Address = address;
        ArrayAddressVariable = addressVar;
        ArrayOffsetVariable = arrayOffset;
        AdressVariable = adressVariable;
        IsSet = true;
        IsArray = true;
        Name = name;
    }
    
    // public Variable(long address, long arrayAddress, long arrOffset)
    // {
    //     Address = address;
    //     ArrayAddress = arrayAddress;
    //     ArrayOffset = arrOffset;
    //     IsSet = true;
    //     IsArray = true;
    // }

    public Variable( long offset, long arrayBeginIdx, long arrayEndIdx, String name= "")
    {
        Address = offset;
        ArrayOffsetVariable = new Variable(arrayBeginIdx);
        
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
    
    public Variable(string name, long address, bool isParameter)
    {
        Name = name;
        Address = address;
        IsSet = true;
        IsParameter = isParameter;
        
    }


    public long? GetValue()
    {
        if (!IsSet)
        {
            Console.WriteLine($"Error: Variable '{Name}' is not set.");
            return null; 
        }

        
        if (IsConst)
        {
            return Value;
        }

        
        if (IsArray && ArrayOffsetVariable != null)
        {
            
            long arrayOffset = ArrayOffsetVariable.GetValue() ?? 0;
            return ArrayAddress + arrayOffset; 
        }

        
        return Value;
    }
    
    
    
}