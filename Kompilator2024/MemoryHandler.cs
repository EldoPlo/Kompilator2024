namespace Kompilator2024
{
    public class MemoryHandler
    {
        private Dictionary<string, Dictionary<string, Symbol>> _memSymbolMap =
            new Dictionary<string, Dictionary<string, Symbol>>();

        private Dictionary<string, Procedure> Procedures = new Dictionary<string, Procedure>();
        private Dictionary<string, Symbol> _constantsDictionary = new Dictionary<string, Symbol>();
        public Stack<string> CalledProcedures = new Stack<string>();
        private Stack<Dictionary<string, Symbol>> _contextStack = new Stack<Dictionary<string, Symbol>>();
        private Dictionary<string, Symbol> _currentContext = new Dictionary<string, Symbol>();
        public List<string> _proceduresOrder = new List<string>();
        public List<(string,int)> _proceduresDeclarationOrder = new List<(string,int)>();
        public List<(string,int)> _proceduresCallOrder = new List<(string,int)>();
        
        
        private List<string> _errors = new List<string>();
        private long _memoryEndPointer = 15;
        private int _tempVariableCounter = 0;
        private bool Errorfound;
        private long ErrorCounter = 0;
        private string ErrorTextColor = "\\u001B31;1m";
        private long currColumn;
        private long currLine;
        public string currentContextName;

        public void AddError(string errorMessage,long line)
        {

            _errors.Add($"Error on line '{line}: {errorMessage}");
            Errorfound = true;
            ErrorCounter++;
        }
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

            return new Symbol {Name = "validsymbol" };
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

        public Variable GetVariable(string name, bool isInitialazing = true)
        {
            if (_currentContext.TryGetValue(name, out var symbol))
            {
                if (symbol.isArray)
                {
                    AddError($"Cannot use '{name}' as variable in line '{currLine}' : '{currColumn}'", currLine);
                    return GetInvalidVariable();
                }

                if (!isInitialazing && !symbol.isInitialized)
                {
                    AddError($"Not initialized symbol '{name}' in '{currLine}' : '{currColumn}'",currLine );
                    return GetInvalidVariable();
                }
                if (isInitialazing)
                {
                    symbol.isInitialized = true;
                }
                return new Variable(name, symbol.Offset);
            }

            var errname = name;
            GetSymbol(name);
            if (GetSymbol(name).Name == "validsymbol")
            {
                AddError($"Uknown variable '{errname}'", currLine);
                return new Variable("validsymbol");
            }
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

            if (!_currentContext.TryGetValue(name, out var sym))
            {
                AddError($"Array  '{name}' does not exitst", currLine);
                return GetInvalidVariable();
            }

            if (!sym.isArrayy())
            {
                AddError(
                    $"Error '{name}' variable is not in array at line '{currLine}' : '{name}' is declared as casual in line '{currColumn}'",currLine);
                return GetInvalidVariable();
            }

            if (sym.ArrayEndIdx < pos || sym.ArrayBeginIdx > pos)
            {
                AddError($"Error '{name}' ['{pos}'] out of bounds at line '{currLine}' : '{currColumn}'",currLine);
                return GetInvalidVariable();
            }

            Variable ret_pos = GetConstVariable(pos);
            Variable offsetVar = GetConstVariable(sym.ArrayBeginIdx);
            Variable adressVar = GetConstVariable(sym.GotOffset());

            return new Variable(sym.GotOffset(), ret_pos, offsetVar, adressVar, name);
        }

        public Variable GetArrValVar(string name, string pos_var)
        {

            if (!_currentContext.TryGetValue(name, out var sym))
            {
                AddError($"This symbol '{name}' not exist", currLine);
                return GetInvalidVariable();
            }
            
            if (!_currentContext.TryGetValue(pos_var, out var pos))
            {
                AddError($"Array index symbol '{pos_var}' not exist", currLine);
                return GetInvalidVariable();
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

            Variable posVar = GetVariable(pos.Name,false);
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
        
        public void RegisterProcedure(string procName,int line)
        {
            if (!_proceduresDeclarationOrder.Contains((procName,line)))
            {
                _proceduresDeclarationOrder.Add((procName,line));
            }
        }
        
        public void RegisterProcedureCall(string procName, int lineNumber)
        {
            _proceduresCallOrder.Add((procName,lineNumber));
        }


        public void AddFunction(Procedure procedure)
        {
           
            Procedures[procedure.Name] = procedure;
        }

        public Procedure GetFunction(string name)
        {

            if (!CheckIfProcedureExists(name))
            {
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
            _currentContext[name].isInitialized = variable.isInitialized;
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

        public Procedure GetInvalidUndeclaredProcedure()
        {
            if (CheckIfProcedureExists("undeclared"))
            {
                return GetFunction("undeclared");
            }

            return Procedure.InvalidUndeclared();
        }
        
     
       
        
        public void ValidateProcedureOrder()
        {
            int lastIndex = -1;
            for (var i = 0; i < _proceduresCallOrder.Count; i++)
            {
                if (_proceduresCallOrder[i].Item2 < _proceduresDeclarationOrder.First(x => x.Item1.Equals(_proceduresCallOrder[i].Item1)).Item2)
                {
                    AddError($"Procedure '{_proceduresCallOrder[i].Item1}' is called before its declaration!", _proceduresCallOrder[i].Item2);
                }
                
            }
        }
        
       

    }
}