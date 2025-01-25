namespace Kompilator2024
{
    public class Procedure
    {
       
        public string Name { get; }
        public List<string> Parameters { get; }
        public List<string> Declaretions { get; set; }
        public bool IsExecuted { get; private set; }
        public Dictionary<string, Symbol> ExecutionContext { get; private set; }

        public l4Parser.CommandsContext CommandsContext;
        public l4Parser.DeclarationsContext? DeclarationsContext = null;

        public Procedure(string name, List<string> parameters)
        {
            Name = name;
            Parameters = parameters ?? new List<string>();
            IsExecuted = false;
            ExecutionContext = new Dictionary<string, Symbol>();
           
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
