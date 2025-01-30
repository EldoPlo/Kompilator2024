namespace Kompilator2024
{
    public class Procedure
    {
       
        public string Name { get; }
        public List<string> Parameters { get; } = new List<string>();

        public List<string> Declaretions { get; set; } = new List<string>();
        public bool IsExecuted { get; private set; }
        public bool isValid;
        public bool isCalled;
        public bool isUndeclared = false;
        public Dictionary<string, Symbol> ExecutionContext { get; private set; }

        public l4Parser.CommandsContext CommandsContext;
        public l4Parser.DeclarationsContext? DeclarationsContext = null;

        public Procedure(string name, List<string> parameters)
        {
            Name = name;
            Parameters = parameters ?? new List<string>();
            IsExecuted = false;
            ExecutionContext = new Dictionary<string, Symbol>();
            isValid = false;
            isCalled = false;

        }
        public Procedure(string name)
        {
            Name = "invalid";
            isValid = true;
            isCalled = false;
        }
        public static Procedure InvalidCalledProcedure()
        {
            return new Procedure("called") {isValid = true, isCalled = true};
        
        }
        
        public static Procedure InvalidProcedure()
        {
            return new Procedure("invalid"){isValid = true, isCalled = false};
        
        }

        public static Procedure InvalidUndeclared()
        {
            return new Procedure("undeclared") { isValid = true, isCalled = false, isUndeclared = true};
        }
      
        
    }
}
