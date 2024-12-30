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
    

    public LanguageVisitor(MemoryHandler memoryHandler, CodeGenerator codeGenerator)
    {
        _memoryHandler = memoryHandler;
        _codeGenerator = codeGenerator;
    }

    public override VisitorDataTransmiter VisitWithProcedures(l4Parser.WithProceduresContext ctx)
    {
        var dataTransmiter = new VisitorDataTransmiter();
        
        var proc_ret = Visit(ctx.procedures());
        dataTransmiter.CodeBuilder.Append(proc_ret.CodeBuilder);
            
        var main_ret = Visit(ctx.main());
        dataTransmiter.CodeBuilder.Append(main_ret.CodeBuilder);
        
        _codeGenerator.End(dataTransmiter.CodeBuilder);
        return dataTransmiter;
    }

    public override VisitorDataTransmiter VisitNoPreocedures(l4Parser.NoPreoceduresContext ctx)
    {
        var dataTransmiter = new VisitorDataTransmiter();
        _codeGenerator.InitConstants(dataTransmiter.CodeBuilder);
        _memoryHandler.InitConstantVariables();
        var main_ret = Visit(ctx.main());
        dataTransmiter.CodeBuilder.Append(main_ret.CodeBuilder);
        
        _codeGenerator.End(dataTransmiter.CodeBuilder);
        return dataTransmiter;    }

    public override VisitorDataTransmiter VisitDeclare(l4Parser.DeclareContext ctx)
    {
        var decRet = Visit(ctx.declarations());
        var com_ret = Visit(ctx.commands());

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
        Visit(ctx.declarations());
        _memoryHandler.SetLocation(ctx.PIDENTIFIER().Symbol.Line, ctx.PIDENTIFIER().Symbol.Column);
        _memoryHandler.GetSymbol(ctx.PIDENTIFIER().GetText());
        

        return new VisitorDataTransmiter();

    }
    
    public override VisitorDataTransmiter VisitPutSymbol2(l4Parser.PutSymbol2Context ctx)
    {
        _memoryHandler.SetLocation(ctx.PIDENTIFIER().Symbol.Line, ctx.PIDENTIFIER().Symbol.Column);
        _memoryHandler.GetSymbol(ctx.PIDENTIFIER().GetText());
       

        return new VisitorDataTransmiter();
    }

    public override VisitorDataTransmiter VisitPutTable1(l4Parser.PutTable1Context ctx)
    {
        var ret = Visit(ctx.declarations());
        _memoryHandler.SetLocation(ctx.PIDENTIFIER().Symbol.Line, ctx.PIDENTIFIER().Symbol.Column);
        var retLeft = GenerateNum(long.Parse(ctx.left.Text));
        ret.CodeBuilder.Append(retLeft.CodeBuilder);
        
        var tableAdd = _memoryHandler.AddTable(ctx.PIDENTIFIER().GetText(), long.Parse(ctx.left.Text), long.Parse(ctx.right.Text));
       
        var retAddress = GenerateNum(tableAdd);
        ret.CodeBuilder.Append(retAddress.CodeBuilder);

        return ret;
    }
    
    public override VisitorDataTransmiter VisitPutTable2(l4Parser.PutTable2Context ctx)
    {
        _memoryHandler.SetLocation(ctx.PIDENTIFIER().Symbol.Line, ctx.PIDENTIFIER().Symbol.Column);
        var retLeft = GenerateNum(long.Parse(ctx.left.Text));

        var tableAdd = _memoryHandler.AddTable(ctx.PIDENTIFIER().GetText(), long.Parse(ctx.left.Text), long.Parse(ctx.right.Text));

        var retAddress = GenerateNum(tableAdd);
        retLeft.CodeBuilder.Append(retAddress.CodeBuilder);
        
        return retLeft;
    }


    public override VisitorDataTransmiter VisitWrite(l4Parser.WriteContext ctx)
    {
        Console.WriteLine("Znaleziono komendę WRITE");

        var valueContext = Visit(ctx.value());
        var dataTransmiter = new VisitorDataTransmiter();

        if (valueContext is null)
        {
            Console.WriteLine("Wartość w WRITE nie jest poprawna.");
            return dataTransmiter;
        }


        var variable = valueContext.Variable;
        _codeGenerator.Write(variable, dataTransmiter.CodeBuilder);

        return dataTransmiter;
    }
    
    public override VisitorDataTransmiter VisitRead(l4Parser.ReadContext ctx)
    {
        var valueContext = Visit(ctx.identifier());
        var dataTransmiter = new VisitorDataTransmiter();
        if (valueContext is null)
        {
            Console.WriteLine("Wartość w Read nie jest poprawna.");
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
        
        
     

        

        // Wywołaj metodę Multiply w celu wygenerowania kodu mnożenia
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
        var identifierDataTransmitter = Visit(ctx.identifier());
        var expressionDataTransmiter = Visit(ctx.expression());
        var dataTransmiter = new VisitorDataTransmiter();

        if (identifierDataTransmitter != null && expressionDataTransmiter != null)
        {
            var variable = identifierDataTransmitter.Variable;

            dataTransmiter.CodeBuilder.Append(expressionDataTransmiter.CodeBuilder);

            var offset = _codeGenerator.Assign(variable,  expressionDataTransmiter.Variable, dataTransmiter.CodeBuilder);

            dataTransmiter.Offset += offset;
            return dataTransmiter;
        }
        else
        {
            Console.WriteLine("Błąd: Brak identyfikatora lub wyrażenia dla przypisania.");
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

    public override VisitorDataTransmiter VisitId(l4Parser.IdContext ctx)
    {
        return Visit(ctx.identifier());
    }

 

    public override VisitorDataTransmiter VisitGetPIDENTIFIER(l4Parser.GetPIDENTIFIERContext ctx)
    {
        _memoryHandler.SetLocation(ctx.PIDENTIFIER().Symbol.Line, ctx.PIDENTIFIER().Symbol.Column);
        lastPID = ctx.PIDENTIFIER().GetText();
        return new VisitorDataTransmiter(_memoryHandler.GetVariable(ctx.PIDENTIFIER().GetText()));
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
        if (_memoryHandler.CheckIfSymbolExists(numVal.ToString()))
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
}
