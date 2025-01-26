namespace Kompilator2024
{
    public class MemoryHandler
    {
        private Dictionary<string, Dictionary<string, Symbol>> _memSymbolMap = new Dictionary<string, Dictionary<string, Symbol>>();
        private Dictionary<string, Procedure> Procedures = new Dictionary<string, Procedure>();
        private HashSet<string> CalledProcedures = new HashSet<string>();
        private Stack<Dictionary<string, Symbol>> _contextStack = new Stack<Dictionary<string, Symbol>>(); 
        private Dictionary<string, Symbol> _currentContext = new Dictionary<string, Symbol>(); 

        private long _memoryEndPointer = 15;
        private int _tempVariableCounter = 0;
        private bool Errorfound;
        private long ErrorCounter;
        private string ErrorTextColor = "\\u001B31;1m";
        private long currColumn;
        private long currLine;

        public bool CheckIfSymbolExists( string name)
        {
            return _currentContext.ContainsKey(name);
        }

        public bool CheckIfProcedureExists(string procname)
        {
            return Procedures.ContainsKey(procname);
        }

        public long GetSymbolOffset( string symbol)
        {
            if (CheckIfSymbolExists(symbol))
            {
                Console.WriteLine($"Symbol '{symbol}' is already defined.");
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
                Console.WriteLine($"Symbol '{name}' is already defined.");
                return _currentContext[name];
            }

            throw new Exception($"This Symbol '{name}'is not exist");
        }
        
        public long GetTable(string name, long begin, long end)
        {
            if (CheckIfSymbolExists(name))
            {
                Console.WriteLine($"{ErrorTextColor}Table '{name}' is already defined at {currLine}:{currColumn}.\u001B[0m");
                Errorfound = true;
                ErrorCounter++;
                return -1;
            }

            if (end < begin)
            {
                Console.WriteLine($"{ErrorTextColor}Error '{name}' endIndex is smaller than beginIndex.\u001B[0m");
                Errorfound = true;
                ErrorCounter++;
                return -1;
            }

            long offset = end - begin + 1;
            _currentContext.Add(name, new Symbol(name, offset, begin, end));
            return offset;
        }

        

        public void InitConstantVariables(string context)
        {
            
            _memSymbolMap[context].Add("1", new Symbol("1", 11));
            _memSymbolMap[context].Add("0", new Symbol("0", 10));
        }

        public Variable GetConstVariable( long value)
        {
            var name = $"{value}";

            if (CheckIfSymbolExists( name))
            {
                return new Variable(name, _currentContext[name].Offset, value);
            }

            var offset = AllocMemorySingle();
            var sym = new Symbol(name, offset);
            _currentContext.Add(name, sym);
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
                return new Variable(name, symbol.Offset);
            }

            
            GetSymbol(name);
            return new Variable(name, _currentContext[name].Offset);
        }
        public Variable GetIterator( string name)
        {
            if (CheckIfSymbolExists(name))
            {
                if (_currentContext[name].IsIterator())
                {
                    return new Variable(name, _currentContext[name].GotOffset());
                }

                Console.WriteLine(ErrorTextColor + " " + name + " already defined non-iterator with this name " + currLine + ":" + currColumn + ".");
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
                Console.WriteLine($"{ErrorTextColor}Table '{name}' is already defined  at {currLine}:{currColumn}.\u001B[0m");
                Errorfound = true;
                ErrorCounter++;
                return -1;
            }

            if (end < begin)
            {
                Console.WriteLine($"{ErrorTextColor}Error '{name}' endIndex is smaller than beginIndex.\u001B[0m");
                Errorfound = true;
                ErrorCounter++;
                return -1;
            }

            long offset = AllocMemoryArr(end - begin + 1);
            _currentContext.Add(name, new Symbol(name, offset, begin, end));
            return offset;
        }

        public Variable GetArrValNum( string name, long pos)
        {
            Symbol sym = _currentContext[name];

            if (sym == null)
            {
                return null;
            }

            if (!sym.isArrayy())
            {
                Console.WriteLine($"{ErrorTextColor}Error '{name}' variable is not in array '{currLine} : '{currColumn}'.\u001B[0m");
                Errorfound = true;
                ErrorCounter++;
                return null;
            }

            if (sym.ArrayEndIdx < pos || sym.ArrayBeginIdx > pos)
            {
                Console.WriteLine($"{ErrorTextColor}Error '{name}' ['{pos}'] out of bounds in '{currLine} : '{currColumn}'.\u001B[0m");
                Errorfound = true;
                ErrorCounter++;
                return null;
            }

            Variable ret_pos = GetConstVariable( pos);
            Variable offsetVar = GetConstVariable(sym.ArrayBeginIdx);
            Variable adressVar = GetConstVariable(sym.GotOffset());

            Variable ret = new Variable(sym.GotOffset(), ret_pos, offsetVar, adressVar);
            return ret;
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
                Console.WriteLine($"{ErrorTextColor}Error '{name}' variable is not in array '{currLine} : '{currColumn}'.\u001B[0m");
                Errorfound = true;
                ErrorCounter++;
                return null;
            }
            else if (pos.isArrayy())
            {
                Console.WriteLine($"{ErrorTextColor}Error '{name}' position symbol is in array '{currLine} : '{currColumn}'.\u001B[0m");
                Errorfound = true;
                ErrorCounter++;
                return null;
            }

            Variable posVar = GetVariable(pos.Name);
            Variable offsetVar = GetConstVariable( sym.ArrayBeginIdx);
            Variable adressVar = GetConstVariable( sym.GotOffset());

            Variable ret = new Variable(sym.GotOffset(), posVar, offsetVar, adressVar);
            return ret;
        }

        public Variable CopyVariable( Variable var)
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

        public void RemoveVariable( Variable var)
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
                throw new Exception($"Function '{funcName}' is already declared!");
            }

            _memSymbolMap[funcName] = new Dictionary<string, Symbol>();
            InitConstantVariables(funcName);
        }


        public void AddFunction(Procedure procedure)
        {
            Procedures[procedure.Name] = procedure;
        }

        public Procedure GetFunction(string name)
        {
            if (!CheckIfProcedureExists(name))
            {
                throw new Exception($"Function '{name} doesn't exist");
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
            _memSymbolMap[name].Clear();
            _currentContext.Clear();
            InitConstantVariables(name);
        }
    }
}
