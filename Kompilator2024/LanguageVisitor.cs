using System.Text;
using Kompilator2024;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Microsoft.VisualBasic.CompilerServices;


namespace Kompilator2024;

public class LanguageVisitor : l4BaseVisitor<VisitorDataTransmiter>
{
    private readonly MemoryHandler _memoryHandler;
    private readonly CodeGenerator _codeGenerator;
    private string lastPID = string.Empty;
    private List<string> currentparrams = new List<string>();
    private List<string> currentdeclarations = new List<string>();
    private List<Symbol> currentargs = new List<Symbol>();
    private List<string> _errors = new List<string>();
    private bool isIninitialaizngCommand = false;
    
    private bool Errorfound;
    private long ErrorCounter=0;
    
    public LanguageVisitor(MemoryHandler memoryHandler, CodeGenerator codeGenerator)
    {
        _memoryHandler = memoryHandler;
        _codeGenerator = codeGenerator;
    }
    public void AddError(string errorMessage)
    {
        
        _errors.Add(errorMessage);  
        Errorfound = true;
        ErrorCounter++;
    }

    public void PrintErrors()
    {
        if (_errors.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var error in _errors)
            {
                Console.WriteLine(error);
            }
            Console.ResetColor();
        }
    }
    public override VisitorDataTransmiter VisitWithProcedures(l4Parser.WithProceduresContext ctx)
    {
        var dataTransmiter = new VisitorDataTransmiter();
        
        var proc_ret = Visit(ctx.procedures());
        dataTransmiter.CodeBuilder.Append(proc_ret.CodeBuilder);
        _memoryHandler.DeclareFunction("main");
        
        _codeGenerator.InitConstants(dataTransmiter.CodeBuilder);
      
        _memoryHandler.SetContext("main");
        var main_ret = Visit(ctx.main());
        dataTransmiter.CodeBuilder.Append(main_ret.CodeBuilder);
        
        _codeGenerator.End(dataTransmiter.CodeBuilder);
        _memoryHandler.ValidateProcedureOrder();
        return dataTransmiter;
    }

    public override VisitorDataTransmiter VisitNoPreocedures(l4Parser.NoPreoceduresContext ctx)
    {
        var dataTransmiter = new VisitorDataTransmiter();
        _memoryHandler.DeclareFunction("main");
        _codeGenerator.InitConstants(dataTransmiter.CodeBuilder);
        
        _memoryHandler.SetContext("main");
        var main_ret = Visit(ctx.main());
        dataTransmiter.CodeBuilder.Append(main_ret.CodeBuilder);
        
        _codeGenerator.End(dataTransmiter.CodeBuilder);
        return dataTransmiter;    
    }

    public override VisitorDataTransmiter VisitProc_head(l4Parser.Proc_headContext ctx)
    {
        var funcName = ctx.PIDENTIFIER().GetText();
        _memoryHandler.DeclareFunction(funcName);
        _memoryHandler.RegisterProcedure(funcName,ctx.Start.Line);
        Visit(ctx.args_decl());
        Procedure procedure = new Procedure(ctx.PIDENTIFIER().GetText(), new List<string>(currentparrams));
        _memoryHandler.AddFunction(procedure);
        currentparrams.Clear();
        return new VisitorDataTransmiter(procedure);
    }

    public override VisitorDataTransmiter VisitArgs_decl(l4Parser.Args_declContext ctx)
    {
        if (ctx.args_decl() is not null)
        {
            Visit(ctx.args_decl());
        }
        
        currentparrams.Add((ctx.T() is null ? "" : "T ") + ctx.PIDENTIFIER().GetText());
        return new VisitorDataTransmiter();
    }

    public override VisitorDataTransmiter VisitProcedures(
        l4Parser.ProceduresContext ctx)
    {
        if (ctx.procedures() is not null)
        {
            Visit(ctx.procedures());
        }
        var dataTransmiter = new VisitorDataTransmiter();
        
        var currentContext = _memoryHandler.GetCurrentContext();
        var procedure = Visit(ctx.proc_head()).Procedure;
        _memoryHandler.SetContext(procedure.Name);
        if(ctx.declarations() is not null)
        { 
            var declaretions = Visit(ctx.declarations());
            procedure.DeclarationsContext = ctx.declarations();
            procedure.Declaretions = new List<string>(currentdeclarations);
           
            foreach (var declaration in procedure.Declaretions)
            {
                
                foreach (var param in procedure.Parameters)
                {
                    var normalizedParam = RemoveTablePrefix(param);

                    if (normalizedParam == declaration)
                    {
                        _memoryHandler.AddError($"Variable '{declaration}' declared in procedure '{procedure.Name}' conflicts with its parameter '{param}' Cannot declare variable with the same name as parameter in one function.",ctx.Start.Line + 1);
                    }
                }
                
            }
            currentdeclarations.Clear();
        }
       
        procedure.CommandsContext = ctx.commands();


        _memoryHandler.SetContext(currentContext);
        return new VisitorDataTransmiter();
    }

public override VisitorDataTransmiter VisitProc_call(l4Parser.Proc_callContext ctx)
{
    var datatransmiter = new VisitorDataTransmiter();
    Visit(ctx.args());
    var funcName = ctx.PIDENTIFIER().GetText();
    var function = _memoryHandler.GetFunction(funcName);
    
    
    _memoryHandler.RegisterProcedureCall(funcName, ctx.Start.Line); 
    if (function.isValid)
    {
        if (!function.isCalled)
        {
            _memoryHandler.AddError($"Attempt to call an undeclared procedure '{ctx.PIDENTIFIER().GetText()}'.", ctx.Start.Line);
        }
        else if (function.isCalled)
        {
            _memoryHandler.AddError($"Recursive call to the procedure '{ctx.PIDENTIFIER().GetText()}'.", ctx.Start.Line);
        }
       
        return datatransmiter;
    }
   
 
   
    _memoryHandler.CalledProcedures.Push(function.Name);
   
    if (currentargs.Count != function.Parameters.Count)
    {
        AddError($"Error : Diffrent quantity of arguments and parameters in function '{function.Name}' in line '{ctx.Start.Line}' ");
        return datatransmiter;
    }

    var currentcontext = _memoryHandler.GetCurrentContext();  
    _memoryHandler.SetContext(function.Name);  
    
    _memoryHandler.ResetContext(function.Name);

    var argscopy = new List<Symbol>(currentargs);
    
    for (int i = 0; i < currentargs.Count; i++)
    {
        var originalParam = function.Parameters[i]; 
        var paramName = originalParam;

        
        if (originalParam.StartsWith("T ")) 
        {
            
            paramName = originalParam.Substring(2);

            
            
            _memoryHandler.GetTable(paramName, currentargs[i].Offset, currentargs[i].ArrayBeginIdx, currentargs[i].ArrayEndIdx); 
        }
      
        else
        {
            
            _memoryHandler.GetSymbolOffset(paramName); 
            _memoryHandler.SetSymbolOffset(paramName, currentargs[i]);
        }


        if (!(_memoryHandler.GetSymbol(paramName).isArray) && currentargs[i].isArray)
        {
            _memoryHandler.AddError($"Invalid args Type during calling calling function '{function.Name}'",ctx.Start.Line);
        }

        
      
    }

 
    currentargs.Clear();

    
    if (function.DeclarationsContext is not null)
    {
        datatransmiter.CodeBuilder.Append(Visit(function.DeclarationsContext).CodeBuilder);
    }

    
    datatransmiter.CodeBuilder.Append(Visit(function.CommandsContext).CodeBuilder);

    _memoryHandler.CalledProcedures.Pop();
    
    for (int i = 0; i < argscopy.Count; i++)
    {
        var originalParam = function.Parameters[i]; 
        var paramName = originalParam;

        if (!originalParam.StartsWith("T "))
        {
            argscopy[i].isInitialized = _memoryHandler.GetCurrentContext()[paramName].isInitialized;
        }

    }

    _memoryHandler.ResetContext(function.Name);
    _memoryHandler.SetContext(currentcontext);

    return datatransmiter;  
}



    public override VisitorDataTransmiter VisitCall(l4Parser.CallContext ctx)
    {
        return Visit(ctx.proc_call());
    }

    public override VisitorDataTransmiter VisitArgs(l4Parser.ArgsContext ctx)
    {
        if (ctx.args() is not null)
        {
            Visit(ctx.args());
        }
        
        var symbol = _memoryHandler.GetSymbol(ctx.PIDENTIFIER().GetText());
        currentargs.Add(symbol);
        return new VisitorDataTransmiter();
    }

    public override VisitorDataTransmiter VisitDeclare(l4Parser.DeclareContext ctx)
    {
        var decRet = Visit(ctx.declarations());
        var com_ret = Visit(ctx.commands());
        currentdeclarations.Clear();
        VisitorDataTransmiter ret = new VisitorDataTransmiter();

        ret.CodeBuilder.Append(decRet.CodeBuilder);
        ret.CodeBuilder.Append(com_ret.CodeBuilder);

        return ret;
        
        
    }

    public override VisitorDataTransmiter VisitNoDeclare(l4Parser.NoDeclareContext ctx)
    {
        var com_ret = Visit(ctx.commands());

        VisitorDataTransmiter ret = new VisitorDataTransmiter();

        ret.CodeBuilder.Append(com_ret.CodeBuilder);

        return ret;
    }

    public override VisitorDataTransmiter VisitCommands(l4Parser.CommandsContext ctx)
    {
        if (ctx.commands() is null)
        {
            return Visit(ctx.command());
        }
        
        var commandsRet = Visit(ctx.commands());
        var commandRet = Visit(ctx.command());

        var ret = new VisitorDataTransmiter();
        
        ret.CodeBuilder.Append(commandsRet.CodeBuilder);
        ret.CodeBuilder.Append(commandRet.CodeBuilder);

        return ret;
    }

    public override VisitorDataTransmiter VisitPutSymbol1(l4Parser.PutSymbol1Context ctx)
    {
        var ret = Visit(ctx.declarations());
        _memoryHandler.SetLocation(ctx.PIDENTIFIER().Symbol.Line, ctx.PIDENTIFIER().Symbol.Column);
        _memoryHandler.GetSymbolOffset(ctx.PIDENTIFIER().GetText());
        currentdeclarations.Add(ctx.PIDENTIFIER().GetText());

        return ret;

    }
    
    public override VisitorDataTransmiter VisitPutSymbol2(l4Parser.PutSymbol2Context ctx)
    {
        _memoryHandler.SetLocation(ctx.PIDENTIFIER().Symbol.Line, ctx.PIDENTIFIER().Symbol.Column);
        _memoryHandler.GetSymbolOffset(ctx.PIDENTIFIER().GetText());
        currentdeclarations.Add(ctx.PIDENTIFIER().GetText());


        return new VisitorDataTransmiter();
    }

    public override VisitorDataTransmiter VisitPutTable1(l4Parser.PutTable1Context ctx)
    {
        var ret = Visit(ctx.declarations());
        _memoryHandler.SetLocation(ctx.PIDENTIFIER().Symbol.Line, ctx.PIDENTIFIER().Symbol.Column);
        var retLeft = GenerateNum(long.Parse(ctx.left.Text));
        var retRight = GenerateNum(long.Parse(ctx.right.Text));
        ret.CodeBuilder.Append(retLeft.CodeBuilder);
        ret.CodeBuilder.Append(retRight.CodeBuilder);
        
        var tableAdd = _memoryHandler.AddTable(ctx.PIDENTIFIER().GetText(), long.Parse(ctx.left.Text), long.Parse(ctx.right.Text));
       
        var retAddress = GenerateNum(tableAdd);
        ret.CodeBuilder.Append(retAddress.CodeBuilder);
        currentdeclarations.Add(ctx.PIDENTIFIER().GetText());

        return ret;
    }
    
    public override VisitorDataTransmiter VisitPutTable2(l4Parser.PutTable2Context ctx)
    {
        _memoryHandler.SetLocation(ctx.PIDENTIFIER().Symbol.Line, ctx.PIDENTIFIER().Symbol.Column);
        var retLeft = GenerateNum(long.Parse(ctx.left.Text));
        var retRight = GenerateNum(long.Parse(ctx.right.Text));
        retLeft.CodeBuilder.Append(retRight.CodeBuilder);
        var tableAdd = _memoryHandler.AddTable(ctx.PIDENTIFIER().GetText(), long.Parse(ctx.left.Text), long.Parse(ctx.right.Text));

        var retAddress = GenerateNum(tableAdd);
        retLeft.CodeBuilder.Append(retAddress.CodeBuilder);
        currentdeclarations.Add(ctx.PIDENTIFIER().GetText());

        return retLeft;
    }


    public override VisitorDataTransmiter VisitWrite(l4Parser.WriteContext ctx)
    {

        var valueContext = Visit(ctx.value());
        var dataTransmiter = new VisitorDataTransmiter();

        if (valueContext is null)
        {
            AddError($"Error in line '{ctx.Start.Line}': Value in command Write is not correct.");
            return dataTransmiter;
        }

        
        var variable = valueContext.Variable;
        _codeGenerator.Write(variable, valueContext.CodeBuilder);

        return valueContext;
    }
    
    public override VisitorDataTransmiter VisitRead(l4Parser.ReadContext ctx)
    {
        isIninitialaizngCommand = true;
        var valueContext = Visit(ctx.identifier());
        isIninitialaizngCommand = false;
        var dataTransmiter = new VisitorDataTransmiter();
        if (valueContext is null)
        {
            AddError($"Error in line '{ctx.Start.Line}': Value in command READ is not correct.");
            return dataTransmiter;
        }
        var variable = valueContext.Variable;
        _codeGenerator.Read(variable, dataTransmiter.CodeBuilder);

        return dataTransmiter;
    }

    public override VisitorDataTransmiter VisitAdd(l4Parser.AddContext ctx)
    {
        var leftvalue = Visit(ctx.left);
        var rightvalue = Visit(ctx.right);
        var dataTransmitter = new VisitorDataTransmiter(new Variable(0));
        dataTransmitter.CodeBuilder.Append(leftvalue.CodeBuilder);
        dataTransmitter.CodeBuilder.Append(rightvalue.CodeBuilder);
        
        var variable1 = leftvalue.Variable;
        var variable2 = rightvalue.Variable;
        
        _codeGenerator.Add(variable1, variable2, dataTransmitter.CodeBuilder);
        return dataTransmitter;
    }

    public override VisitorDataTransmiter VisitSub(l4Parser.SubContext ctx)
    {
        var leftvalue = Visit(ctx.left);
        var rightvalue = Visit(ctx.right);
        var dataTransmitter = new VisitorDataTransmiter(new Variable(0));
        dataTransmitter.CodeBuilder.Append(leftvalue.CodeBuilder);
        dataTransmitter.CodeBuilder.Append(rightvalue.CodeBuilder);
        
        var variable1 = leftvalue.Variable;
        var variable2 = rightvalue.Variable;
        
        _codeGenerator.Sub(variable1, variable2, dataTransmitter.CodeBuilder);
        return dataTransmitter;
    }
    
    public override VisitorDataTransmiter VisitMul(l4Parser.MulContext ctx)
    {
        var leftvalue = Visit(ctx.left);  
        var rightvalue = Visit(ctx.right); 
        var dataTransmitter = new VisitorDataTransmiter(new Variable(0));

        dataTransmitter.CodeBuilder.Append(leftvalue.CodeBuilder);
        dataTransmitter.CodeBuilder.Append(rightvalue.CodeBuilder);
        
        var variable1 = leftvalue.Variable;
        var variable2 = rightvalue.Variable;

        _codeGenerator.Mul(variable1, variable2, dataTransmitter.CodeBuilder);

        return dataTransmitter;
    }

    public override VisitorDataTransmiter VisitDiv(l4Parser.DivContext ctx)
    {
        var leftvalue = Visit(ctx.left);  
        var rightvalue = Visit(ctx.right); 
        var dataTransmitter = new VisitorDataTransmiter(new Variable(0));
        
        dataTransmitter.CodeBuilder.Append(leftvalue.CodeBuilder);
        dataTransmitter.CodeBuilder.Append(rightvalue.CodeBuilder);
        
        var variable1 = leftvalue.Variable;
        var variable2 = rightvalue.Variable;
        
        _codeGenerator.Div(variable1, variable2, dataTransmitter.CodeBuilder);
        return dataTransmitter;
    }
    
    public override VisitorDataTransmiter VisitMod(l4Parser.ModContext ctx)
    {
        var leftvalue = Visit(ctx.left);  
        var rightvalue = Visit(ctx.right); 
        var dataTransmitter = new VisitorDataTransmiter(new Variable(0));
        
        dataTransmitter.CodeBuilder.Append(leftvalue.CodeBuilder);
        dataTransmitter.CodeBuilder.Append(rightvalue.CodeBuilder);
        
        var variable1 = leftvalue.Variable;
        var variable2 = rightvalue.Variable;
        
        _codeGenerator.Mod(variable1, variable2, dataTransmitter.CodeBuilder);
        return dataTransmitter;
    }

   public override VisitorDataTransmiter VisitAssign(l4Parser.AssignContext ctx)
   { 
        isIninitialaizngCommand = true;
        var identifierDataTransmitter = Visit(ctx.identifier());
        isIninitialaizngCommand = false;
        var expressionDataTransmiter = Visit(ctx.expression());
        var dataTransmiter = new VisitorDataTransmiter();

        if (identifierDataTransmitter != null && expressionDataTransmiter != null)
        {
            var variable = identifierDataTransmitter.Variable;

           
            if (_memoryHandler.GetSymbol(variable.Name).IsIterator())
            {
                _memoryHandler.AddError($"Iterator assignment attempt '{_memoryHandler.GetSymbol(variable.Name).Name}' inside loop body",ctx.Start.Line);
            }

           
            dataTransmiter.CodeBuilder.Append(expressionDataTransmiter.CodeBuilder);
            var offset = _codeGenerator.Assign(variable, expressionDataTransmiter.Variable, dataTransmiter.CodeBuilder);

            dataTransmiter.Offset += offset;
            return dataTransmiter;
        }
        else
        {
            
            _memoryHandler.AddError($"Identificator or expression error.",ctx.Start.Line);
        }
        
        return dataTransmiter;
}

    public override VisitorDataTransmiter VisitIf(l4Parser.IfContext ctx)
    {
        VisitorDataTransmiter ex = Visit(ctx.commands());
        VisitorDataTransmiter cond = Visit(ctx.condition());
        VisitorDataTransmiter ret = new VisitorDataTransmiter();
        var offset = ex.CodeBuilder.ToString().Trim().Split("\n", StringSplitOptions.RemoveEmptyEntries).Length;
        ret.Offset += _codeGenerator.If_block(offset, cond.CodeBuilder);
        Console.WriteLine(ret.CodeBuilder);
        ret.CodeBuilder.Append(cond.CodeBuilder);
        ret.CodeBuilder.Append(ex.CodeBuilder);
        return ret;
    }
    
    public override VisitorDataTransmiter VisitIfElse(l4Parser.IfElseContext ctx)
    {
        VisitorDataTransmiter ifblock = Visit(ctx.ifblock);
        VisitorDataTransmiter elseblock = Visit(ctx.elseblock);
        VisitorDataTransmiter cond = Visit(ctx.condition());
        
        VisitorDataTransmiter ret = new VisitorDataTransmiter();
        var offset1 = ifblock.CodeBuilder.ToString().Trim().Split("\n", StringSplitOptions.RemoveEmptyEntries).Length;
        var offset2 = elseblock.CodeBuilder.ToString().Trim().Split("\n", StringSplitOptions.RemoveEmptyEntries).Length;
         _codeGenerator.If_else_block_first(offset1, cond.CodeBuilder);
         _codeGenerator.If_else_block_sec(offset2, ifblock.CodeBuilder);
        

        ret.CodeBuilder.Append(cond.CodeBuilder);
        ret.CodeBuilder.Append(ifblock.CodeBuilder);
        ret.CodeBuilder.Append(elseblock.CodeBuilder);
        return ret;
    }

    public override VisitorDataTransmiter VisitEq(l4Parser.EqContext ctx)
    {
        var leftvalue = Visit(ctx.left);
        var rightvalue = Visit(ctx.right);

        var datatransmiter = new VisitorDataTransmiter();

        datatransmiter.CodeBuilder.Append(leftvalue.CodeBuilder);
        datatransmiter.CodeBuilder.Append(rightvalue.CodeBuilder);

        var variable1 = leftvalue.Variable;
        var variable2 = rightvalue.Variable;

        _codeGenerator.Eq(variable1, variable2, datatransmiter.CodeBuilder);

        return datatransmiter;

    }
    
    public override VisitorDataTransmiter VisitNeq(l4Parser.NeqContext ctx)
    {
        var leftvalue = Visit(ctx.left);
        var rightvalue = Visit(ctx.right);

        var datatransmiter = new VisitorDataTransmiter();

        datatransmiter.CodeBuilder.Append(leftvalue.CodeBuilder);
        datatransmiter.CodeBuilder.Append(rightvalue.CodeBuilder);

        var variable1 = leftvalue.Variable;
        var variable2 = rightvalue.Variable;

        _codeGenerator.Neq(variable1, variable2, datatransmiter.CodeBuilder);

        return datatransmiter;

    }
    
    public override VisitorDataTransmiter VisitGe(l4Parser.GeContext ctx)
    {
        var leftvalue = Visit(ctx.left);
        var rightvalue = Visit(ctx.right);

        var datatransmiter = new VisitorDataTransmiter();

        datatransmiter.CodeBuilder.Append(leftvalue.CodeBuilder);
        datatransmiter.CodeBuilder.Append(rightvalue.CodeBuilder);

        var variable1 = leftvalue.Variable;
        var variable2 = rightvalue.Variable;

        _codeGenerator.Ge(variable1, variable2, datatransmiter.CodeBuilder);

        return datatransmiter;

    }
    
    public override VisitorDataTransmiter VisitLeq(l4Parser.LeqContext ctx)
    {
        var leftvalue = Visit(ctx.left);
        var rightvalue = Visit(ctx.right);

        var datatransmiter = new VisitorDataTransmiter();

        datatransmiter.CodeBuilder.Append(leftvalue.CodeBuilder);
        datatransmiter.CodeBuilder.Append(rightvalue.CodeBuilder);

        var variable1 = leftvalue.Variable;
        var variable2 = rightvalue.Variable;

        _codeGenerator.Leq(variable1, variable2, datatransmiter.CodeBuilder);

        return datatransmiter;

    }
    
    public override VisitorDataTransmiter VisitGeq(l4Parser.GeqContext ctx)
    {
        var leftvalue = Visit(ctx.left);
        var rightvalue = Visit(ctx.right);

        var datatransmiter = new VisitorDataTransmiter();

        datatransmiter.CodeBuilder.Append(leftvalue.CodeBuilder);
        datatransmiter.CodeBuilder.Append(rightvalue.CodeBuilder);

        var variable1 = leftvalue.Variable;
        var variable2 = rightvalue.Variable;

        _codeGenerator.Geq(variable1, variable2, datatransmiter.CodeBuilder);

        return datatransmiter;

    }

    
    public override VisitorDataTransmiter VisitLe(l4Parser.LeContext ctx)
    {
        var leftvalue = Visit(ctx.left);
        var rightvalue = Visit(ctx.right);

        var datatransmiter = new VisitorDataTransmiter();

        datatransmiter.CodeBuilder.Append(leftvalue.CodeBuilder);
        datatransmiter.CodeBuilder.Append(rightvalue.CodeBuilder);

        var variable1 = leftvalue.Variable;
        var variable2 = rightvalue.Variable;

        _codeGenerator.Le(variable1, variable2, datatransmiter.CodeBuilder);
        
        return datatransmiter;

    }


    public override VisitorDataTransmiter VisitWhile(l4Parser.WhileContext ctx)
    {
        VisitorDataTransmiter ex = Visit(ctx.commands());
        VisitorDataTransmiter cond = Visit(ctx.condition());
        VisitorDataTransmiter datatransmiter = new VisitorDataTransmiter();
        
        var offset1 = ex.CodeBuilder.ToString().Trim().Split("\n", StringSplitOptions.RemoveEmptyEntries).Length;
        var offset2 = cond.CodeBuilder.ToString().Trim().Split("\n", StringSplitOptions.RemoveEmptyEntries).Length;
        
        _codeGenerator.While_first_block(offset1, cond.CodeBuilder);
        _codeGenerator.While_sec_block(offset1, offset2, ex.CodeBuilder);

        datatransmiter.CodeBuilder.Append(cond.CodeBuilder);
        datatransmiter.CodeBuilder.Append(ex.CodeBuilder);
        return datatransmiter;
    }
    
    public override VisitorDataTransmiter VisitRepeat(l4Parser.RepeatContext ctx)
    {
        VisitorDataTransmiter ex = Visit(ctx.commands());
        VisitorDataTransmiter cond = Visit(ctx.condition());
        VisitorDataTransmiter datatransmiter = new VisitorDataTransmiter();
        
        var offset1 = ex.CodeBuilder.ToString().Trim().Split("\n", StringSplitOptions.RemoveEmptyEntries).Length;
        var offset2 = cond.CodeBuilder.ToString().Trim().Split("\n", StringSplitOptions.RemoveEmptyEntries).Length;
        
        
        _codeGenerator.Repeat_block(offset1, offset2, cond.CodeBuilder);

        datatransmiter.CodeBuilder.Append(ex.CodeBuilder);
        datatransmiter.CodeBuilder.Append(cond.CodeBuilder);
        return datatransmiter;
    }

    public override VisitorDataTransmiter VisitForUp(l4Parser.ForUpContext ctx)
    {
        var iteratorVar = _memoryHandler.GetIterator(ctx.PIDENTIFIER().GetText());
        VisitorDataTransmiter var1 = Visit(ctx.v1);
        VisitorDataTransmiter var2 = Visit(ctx.v2);
        VisitorDataTransmiter com = Visit(ctx.commands());


        VisitorDataTransmiter dataTransmiter = new VisitorDataTransmiter();
        dataTransmiter.CodeBuilder.Append(var1.CodeBuilder);
        dataTransmiter.CodeBuilder.Append(var2.CodeBuilder);
        var start_var = _memoryHandler.CopyVariable(var1.Variable);
        var end_var = _memoryHandler.CopyVariable(var2.Variable);


        ForLabel label = _memoryHandler.CreateForLabel(iteratorVar, start_var, end_var);
        _codeGenerator.For_block_init(label, var1.Variable, var2.Variable, dataTransmiter.CodeBuilder);

        StringBuilder tempSb = new StringBuilder();
        _codeGenerator.For_to_block_first(label, tempSb);
        var sizeTempSb = _codeGenerator.GetLineCount(tempSb);
        _codeGenerator.For_to_block_second(label, sizeTempSb, com.CodeBuilder);

        _codeGenerator.For_block_addJump(_codeGenerator.GetLineCount(com.CodeBuilder), tempSb);

        dataTransmiter.CodeBuilder.Append(tempSb);
        dataTransmiter.CodeBuilder.Append(com.CodeBuilder);

        _memoryHandler.RemoveVariable(start_var);
        _memoryHandler.RemoveVariable(iteratorVar);
        _memoryHandler.RemoveVariable(end_var);
        
        return dataTransmiter;

    }
    
    public override VisitorDataTransmiter VisitForDown(l4Parser.ForDownContext ctx)
    {
        var iteratorVar = _memoryHandler.GetIterator(ctx.PIDENTIFIER().GetText());
        VisitorDataTransmiter var1 = Visit(ctx.v1);
        VisitorDataTransmiter var2 = Visit(ctx.v2);
        VisitorDataTransmiter com = Visit(ctx.commands());


        VisitorDataTransmiter dataTransmiter = new VisitorDataTransmiter();
        dataTransmiter.CodeBuilder.Append(var1.CodeBuilder);
        dataTransmiter.CodeBuilder.Append(var2.CodeBuilder);
        var start_var = _memoryHandler.CopyVariable(var1.Variable);
        var end_var = _memoryHandler.CopyVariable(var2.Variable);


        ForLabel label = _memoryHandler.CreateForLabel(iteratorVar, start_var, end_var);
        _codeGenerator.For_block_init(label, var1.Variable, var2.Variable, dataTransmiter.CodeBuilder);

        StringBuilder tempSb = new StringBuilder();
        _codeGenerator.For_downto_block_first(label, tempSb);
        var sizeTempSb = _codeGenerator.GetLineCount(tempSb);
        _codeGenerator.For_downto_block_second(label, sizeTempSb, com.CodeBuilder);

        _codeGenerator.For_block_addJump(_codeGenerator.GetLineCount(com.CodeBuilder), tempSb);

        dataTransmiter.CodeBuilder.Append(tempSb);
        dataTransmiter.CodeBuilder.Append(com.CodeBuilder);

        _memoryHandler.RemoveVariable(start_var);
        _memoryHandler.RemoveVariable(iteratorVar);
        _memoryHandler.RemoveVariable(end_var);
        
        return dataTransmiter;

    }


    public override VisitorDataTransmiter VisitEval(l4Parser.EvalContext ctx)
    {
        var valRet = Visit(ctx.value());
        var ret = new VisitorDataTransmiter(valRet.Variable);

        if (valRet.Variable.IsConst)
        {
            ret.CodeBuilder.Append(valRet.CodeBuilder);
            return ret;
        }

        _codeGenerator.GetVarVal(valRet.Variable, 0, ret.CodeBuilder);

        return ret;
    }


    public override VisitorDataTransmiter VisitNum(l4Parser.NumContext ctx)
    {
        var numValue = long.Parse(ctx.NUM().GetText());
        return GenerateNum(numValue);
    }

    public override VisitorDataTransmiter VisitNegNum(l4Parser.NegNumContext ctx)
    {
        var negnumValue = long.Parse(ctx.NUM().GetText());
        return GenerateNum(-negnumValue);

    }
    

    public override VisitorDataTransmiter VisitId(l4Parser.IdContext ctx)
    {
        return Visit(ctx.identifier());
    }

 

    public override VisitorDataTransmiter VisitGetPIDENTIFIER(l4Parser.GetPIDENTIFIERContext ctx)
    {
        _memoryHandler.SetLocation(ctx.PIDENTIFIER().Symbol.Line, ctx.PIDENTIFIER().Symbol.Column);
        lastPID = ctx.PIDENTIFIER().GetText();
       
        return new VisitorDataTransmiter(_memoryHandler.GetVariable(ctx.PIDENTIFIER().GetText(),isIninitialaizngCommand));
    }

    public override VisitorDataTransmiter VisitGetArrayByPid(l4Parser.GetArrayByPidContext ctx)
    {
        
        var pidList = ctx.PIDENTIFIER();
        _memoryHandler.SetLocation(pidList[0].Symbol.Line,pidList[0].Symbol.Column);
        lastPID = pidList[1].GetText();
        return new VisitorDataTransmiter(_memoryHandler.GetArrValVar(pidList[0].GetText(), pidList[1].GetText()));
    }

    public override VisitorDataTransmiter VisitGetArrayByNum(l4Parser.GetArrayByNumContext ctx)
    
    {
        _memoryHandler.SetLocation(ctx.PIDENTIFIER().Symbol.Line, ctx.PIDENTIFIER().Symbol.Column);
        lastPID = ctx.PIDENTIFIER().GetText();
        return new VisitorDataTransmiter(_memoryHandler.GetArrValNum(ctx.PIDENTIFIER().GetText(),int.Parse(ctx.NUM().GetText())));
    }

    private VisitorDataTransmiter GenerateNum(long numVal)
    {
        if (_memoryHandler.CheckIfConstantSymbolExists(numVal.ToString()))
        {
            var var = _memoryHandler.GetConstVariable(numVal);
            var ret_existing =  new VisitorDataTransmiter(var);
            _codeGenerator.LoadValue(var, ret_existing.CodeBuilder);
            return ret_existing;
        }
        
        var variable = _memoryHandler.GetConstVariable(numVal);
        var ret = new VisitorDataTransmiter(variable);
        _codeGenerator.GetConstToMemory(numVal, variable.Address, ret.CodeBuilder);

        return ret;
    }
    private string RemoveTablePrefix(string name)
    {
        return name.StartsWith("T ") ? name.Substring(2) : name;
    }
    public List<string> GetErrors()
    {
        return _errors;
    }
}