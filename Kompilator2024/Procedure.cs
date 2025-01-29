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

        public Procedure(string name, bool init = true)
        {
            Name = "called";
            isValid = true;
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
        // public void AddLocalVariable(Variable variable)
        // {
        //     if (LocalVariables.Exists(v => v.Name == variable.Name))
        //     {
        //         throw new InvalidOperationException($"Variable '{variable.Name}' already exists in procedure '{Name}'.");
        //     }
        //
        //     LocalVariables.Add(variable);
        // }

        public void MarkAsExecuted(Dictionary<string, Symbol> context)
        {
            IsExecuted = true;
            ExecutionContext = context;
        }

        public bool ISValid()
        {
            return isValid;
        }
        //
        // public List<string> GetParameterNames()
        // {
        //     return Parameters.Select(p => p.Name).ToList();
        // }
        //
        // public bool HasLocalVariable(string name)
        // {
        //     return LocalVariables.Any(v => v.Name == name);
        // }

    }
}
