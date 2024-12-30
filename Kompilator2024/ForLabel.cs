namespace Kompilator2024;

public class ForLabel
{
    public Variable Iterator { get; set; }
    public Variable Start { get;  set; }
    public Variable End { get; set; }

    public ForLabel(Variable iterator, Variable start, Variable end)
    {
        Iterator = iterator;
        Start = start;
        End = end;
    }
}