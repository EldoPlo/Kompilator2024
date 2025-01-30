using System.Text;

namespace Kompilator2024;

public class VisitorDataTransmiter
{
    public Variable Variable { get;  set; } = new Variable("default", 0);
    public Procedure Procedure { get; set; } 
    public StringBuilder CodeBuilder { get; set; } = new StringBuilder();

    public long Offset = 0;
    
    public VisitorDataTransmiter() { }

    public VisitorDataTransmiter(Variable var)
    {
        Variable = var;
    }

    public VisitorDataTransmiter(Procedure procedure)
    {
        Procedure = procedure;
    }
    
}