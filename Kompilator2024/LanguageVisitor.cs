using Kompilator2024;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

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

        var main_ret = Visit(ctx.main());
        dataTransmiter.CodeBuilder.Append(main_ret.CodeBuilder);
        
        _codeGenerator.End(dataTransmiter.CodeBuilder);
        return dataTransmiter;    }

    public override VisitorDataTransmiter VisitDeclare(l4Parser.DeclareContext ctx)
    {
        var dec_ret = Visit(ctx.declarations());
        var com_ret = Visit(ctx.commands());
        return com_ret;
    }

    public override VisitorDataTransmiter VisitNoDeclare(l4Parser.NoDeclareContext ctx)
    {
        return Visit(ctx.commands());
    }

    public override VisitorDataTransmiter VisitCommands(l4Parser.CommandsContext ctx)
    {
        if (ctx.commands() is null)
        {
            return Visit(ctx.command());
        }

        var commandRet = Visit(ctx.command());
        var commandsRet = Visit(ctx.commands());

        var ret = new VisitorDataTransmiter();

        ret.CodeBuilder.Append(commandsRet.CodeBuilder);
        ret.CodeBuilder.Append(commandRet.CodeBuilder);

        return ret;
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
        var dataTransmitter = new VisitorDataTransmiter();
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
        var dataTransmitter = new VisitorDataTransmiter();
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
        var dataTransmitter = new VisitorDataTransmiter();
        
        
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
        var dataTransmitter = new VisitorDataTransmiter();
        
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
        var dataTransmitter = new VisitorDataTransmiter();
        
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

            var offset = _codeGenerator.Assign(variable, dataTransmiter.CodeBuilder);

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


    public override VisitorDataTransmiter VisitEval(l4Parser.EvalContext ctx)
    {
        var valRet = Visit(ctx.value());
        var ret = new VisitorDataTransmiter();

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
        if (_memoryHandler.CheckIfSymbolExists(numValue.ToString()))
        {
            return new VisitorDataTransmiter(_memoryHandler.GetConstVariable(numValue));
        }
        
        var variable = _memoryHandler.GetConstVariable(numValue);
        var ret = new VisitorDataTransmiter(variable);
        _codeGenerator.GetConstToMemory(numValue, variable.Address, ret.CodeBuilder);
        
        return ret;
    }

    public override VisitorDataTransmiter VisitId(l4Parser.IdContext ctx)
    {
        return Visit(ctx.identifier());
    }

    public override VisitorDataTransmiter VisitIdentifier(l4Parser.IdentifierContext ctx)
    {
        var variableName = ctx.PIDENTIFIER().GetText();
        lastPID = variableName;

        var variable = _memoryHandler.GetVariable(variableName);
        return new VisitorDataTransmiter(variable);
    }

   
}
