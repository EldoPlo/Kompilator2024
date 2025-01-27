namespace Kompilator2024
{
    public class MemoryHandler
    {
        private Dictionary<string, Dictionary<string, Symbol>> _memSymbolMap =
            new Dictionary<string, Dictionary<string, Symbol>>();

        private Dictionary<string, Procedure> Procedures = new Dictionary<string, Procedure>();
        private Dictionary<string, Symbol> _constantsDictionary = new Dictionary<string, Symbol>();
        public HashSet<string> CalledProcedures = new HashSet<string>();
        private Stack<Dictionary<string, Symbol>> _contextStack = new Stack<Dictionary<string, Symbol>>();
        private Dictionary<string, Symbol> _currentContext = new Dictionary<string, Symbol>();
        
        private List<string> _errors = new List<string>();
        private long _memoryEndPointer = 15;
        private int _tempVariableCounter = 0;
        private bool Errorfound;
        private long ErrorCounter = 0;
        private string ErrorTextColor = "\\u001B31;1m";
        private long currColumn;
        private long currLine;

        public void AddError(string errorMessage,long line)
        {

            _errors.Add($"Error on line '{line}: {errorMessage}");
            Errorfound = true;
            ErrorCounter++;
        }

        // public void PrintErrors()
        // {
        //     if (_errors.Count > 0)
        //     {
        //         Console.ForegroundColor = ConsoleColor.Red;
        //         foreach (var error in _errors)
        //         {
        //             Console.WriteLine(error);
        //         }
        //         Console.ResetColor();
        //     }
        // }
        public MemoryHandler()
        {
            InitConstantVariables();
        }

        public bool CheckIfSymbolExists(string name)
        {
            return _currentContext.ContainsKey(name);
        }

        public bool CheckIfProcedureIsCalled(string name)
        {
            return CalledProcedures.Contains(name);
        }

        public bool CheckIfConstantSymbolExists(string name)
        {
            return _constantsDictionary.ContainsKey(name);
        }

        public bool CheckIfProcedureExists(string procname)
        {
            return Procedures.ContainsKey(procname);
        }

        public long GetSymbolOffset(string symbol)
        {
            if (CheckIfSymbolExists(symbol))
            {
                return _currentContext[symbol].Offset;
            }

            long offset = AllocMemorySingle();
            _currentContext.Add(symbol, new Symbol(symbol, offset));
            return offset;
        }

        public Symbol GetSymbol(string name)
        {
            if (CheckIfSymbolExists(name))
            {
                return _currentContext[name];
            }

            if (CheckIfConstantSymbolExists(name))
            {
                return _constantsDictionary[name];
            }

            throw new Exception($"This Symbol '{name}'is not exist in '{currLine}");
        }

        public void GetTable(string name, long offset, long begin, long end)
        {
            if (CheckIfSymbolExists(name))
            {
                AddError($"Table '{name}' is already defined",currLine);
                return;
            }

            if (end < begin)
            {
                AddError($"Error '{name}' endIndex is smaller than beginIndex.", currLine);
                return;
            }

            _currentContext.Add(name, new Symbol(name, offset, begin, end));
        }

        public void InitConstantVariables()
        {
            _constantsDictionary.Add("1", new Symbol("1", 11));
            _constantsDictionary.Add("0", new Symbol("0", 10));
        }

        public Variable GetConstVariable(long value)
        {
            var name = $"{value}";

            if (CheckIfConstantSymbolExists(name))
            {
                return new Variable(name, _constantsDictionary[name].Offset, value);
            }

            var offset = AllocMemorySingle();
            var sym = new Symbol(name, offset);
            _constantsDictionary.Add(name, sym);
            return new Variable(name, offset, value);
        }

        private long AllocMemorySingle()
        {
            _memoryEndPointer++;
            return _memoryEndPointer - 1;
        }

        public void SetLocation(long line, long column)
        {
            currColumn = column;
            currLine = line;
        }

        public Variable GetVariable(string name)
        {
            if (_currentContext.TryGetValue(name, out var symbol))
            {
                if (symbol.isArray)
                {
                    AddError($"Cannot use '{name}' as variable in line '{currLine}' : '{currColumn}", currLine);
                }

                return new Variable(name, symbol.Offset);
            }


            GetSymbol(name);
            return new Variable(name, _currentContext[name].Offset);
        }

        public Variable GetIterator(string name)
        {
            if (CheckIfSymbolExists(name))
            {
                if (_currentContext[name].IsIterator())
                {
                    return new Variable(name, _currentContext[name].Offset);
                }

                AddError($"'{name}' already defined as a non-iterator at line '{currLine}' : '{currColumn}'",currLine);
                Errorfound = true;
                ErrorCounter++;
                return new Variable(-1);
            }

            long offset = AllocMemorySingle();
            var sym = new Symbol(name, offset, true, true);
            _currentContext.Add(name, sym);
            return new Variable(name, offset);
        }

        public ForLabel CreateForLabel(Variable iterator, Variable start, Variable end)
        {
            var symbol = _currentContext[iterator.Name];
            if (!symbol.isIterator)
            {
                AddError($"'{iterator.Name} is not an iterator",currLine);
            }

            return new ForLabel(iterator, start, end);
        }

        private long AllocMemoryArr(long size)
        {
            long beginPointer = _memoryEndPointer;
            _memoryEndPointer += size;
            return beginPointer;
        }

        public long AddTable(string name, long begin, long end)
        {
            if (CheckIfSymbolExists(name))
            {

                AddError($"Table '{name}' is already defined  at line '{currLine}': '{currColumn}'",currLine);
                Errorfound = true;
                ErrorCounter++;
                return -1;
            }

            if (end < begin)
            {
                AddError($"Error '{name}' endIndex is smaller than beginIndex at line '{currLine}': '{currColumn}.",currLine);
                Errorfound = true;
                ErrorCounter++;
                return -1;
            }

            long offset = AllocMemoryArr(end - begin + 1);
            _currentContext.Add(name, new Symbol(name, offset, begin, end));
            return offset;
        }

        public Variable GetArrValNum(string name, long pos)
        {
            Symbol sym = _currentContext[name];

            if (sym == null)
            {
                return null;
            }

            if (!sym.isArrayy())
            {
                AddError(
                    $"Error '{name}' variable is not in array at line '{currLine}' : '{name}' is declared as casual in line '{currColumn}'",currLine);
                return null;
            }

            if (sym.ArrayEndIdx < pos || sym.ArrayBeginIdx > pos)
            {
                AddError($"Error '{name}' ['{pos}'] out of bounds at line '{currLine}' : '{currColumn}'",currLine);
                return null;
            }

            Variable ret_pos = GetConstVariable(pos);
            Variable offsetVar = GetConstVariable(sym.ArrayBeginIdx);
            Variable adressVar = GetConstVariable(sym.GotOffset());

            return new Variable(sym.GotOffset(), ret_pos, offsetVar, adressVar, name);
        }

        public Variable GetArrValVar(string name, string pos_var)
        {
            Symbol sym = _currentContext[name];
            Symbol pos = _currentContext[pos_var];

            if (sym == null || pos == null)
            {
                return null;
            }

            if (!sym.isArrayy())
            {
                AddError($"Error '{name}' variable is not in array at line '{currLine} : '{currColumn}'.",currLine);
                return GetInvalidVariable();
            }
            else if (pos.isArrayy())
            {
                AddError($"Error '{name}' position symbol is in array at line '{currLine} : '{currColumn}'.",currLine);
                return GetInvalidVariable();
            }

            Variable posVar = GetVariable(pos.Name);
            Variable offsetVar = GetConstVariable(sym.ArrayBeginIdx);
            Variable adressVar = GetConstVariable(sym.GotOffset());

            return new Variable(sym.GotOffset(), posVar, offsetVar, adressVar, name);
        }

        public Variable CopyVariable(Variable var)
        {
            var name = $"{var.Name}-Copy";
            if (CheckIfSymbolExists(name))
            {
                name += "-Copy";
            }

            var offset = AllocMemorySingle();
            _currentContext.Add(name, new Symbol(name, offset, true, true));
            return new Variable(name, offset);
        }

        public void RemoveVariable(Variable var)
        {
            if (!CheckIfSymbolExists(var.Name))
            {
                return;
            }

            _currentContext.Remove(var.Name);
        }

        public void DeclareFunction(string funcName)
        {
            if (CheckIfProcedureExists(funcName))
            {
                AddError($"Function '{funcName}' is already declared!",currLine);
                return;
            }

            _memSymbolMap[funcName] = new Dictionary<string, Symbol>();
            
        }


        public void AddFunction(Procedure procedure)
        {
            Procedures[procedure.Name] = procedure;
        }

        public Procedure GetFunction(string name)
        {

            if (!CheckIfProcedureExists(name))
            {
               // AddError($"Błąd: Próba wywołania niezadeklarowanej procedury '{name}'.");
                return GetInvalidProcedure();
            }

            
            if (CalledProcedures.Contains(name))
            {
                return GetInvalidCalledProcedure();
            }
            return Procedures[name];
        }

        public void SetContext(string context)
        {
            if (!_memSymbolMap.ContainsKey(context))
            {
                throw new Exception($"Function '{context} in not declared");
            }
            
            _currentContext = _memSymbolMap[context];
        }

        public void SetContext(Dictionary<string, Symbol> context)
        {
            _currentContext = context;
        }

        public Dictionary<string, Symbol> GetCurrentContext()
        {
            return _currentContext;
        }

        public void SetSymbolOffset(string name, Symbol variable)
        {
            _currentContext[name].Offset = variable.Offset;
        }

        public void ResetContext(string name)
        {
            if (!_memSymbolMap.ContainsKey(name))
            {
                AddError($"Context '{name}' is not declared",currLine);
                return;
            }

            _memSymbolMap[name].Clear();
            _currentContext.Clear();
        }

        public List<string> GetErrors()
        {
            return _errors;
        }

        public Variable GetInvalidVariable()
        {
            if (CheckIfSymbolExists("invalid"))
            {
                return GetVariable("invalid");
            }

            return Variable.InvalidVariable(GetSymbolOffset("invalid"));
        }

        public Procedure GetInvalidProcedure()
        {
            if (CheckIfProcedureExists("invalid"))
            {
                return GetFunction("invalid");
            }

            return Procedure.InvalidProcedure();
        }
        
        public Procedure GetInvalidCalledProcedure()
        {
            if (CheckIfProcedureExists("called"))
            {
                return GetFunction("called");
            }

            return Procedure.InvalidCalledProcedure();
        }
    }
}