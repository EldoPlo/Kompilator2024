grammar l4;
WS: [ \t\r\n]+ -> skip;
WHITESPACE: ('\t' | ' ' | '\r' | '\n'| '\u000C')+ ;
PIDENTIFIER: [_a-z]+;
NUM : '-'?[0-9]+;
PROCEDURE: 'PROCEDURE';
IS: 'IS';
BEGIN: 'BEGIN';
END: 'END';
PROGRAM: 'PROGRAM';
IF: 'IF';
WHILE: 'WHILE';
FOR: 'FOR';
REPEAT: 'REPEAT';
THEN: 'THEN';
ELSE: 'ELSE';
ENDIF: 'ENDIF';
DO: 'DO';
ENDWHILE: 'ENDWHILE';
UNTIL: 'UNTIL';
FROM: 'FROM';
ENDFOR: 'ENDFOR';
TO: 'TO';
DOWNTO: 'DOWNTO';
READ: 'READ';
WRITE: 'WRITE';
T: 'T';

program_all  : procedures main #WithProcedures
             | main #NoPreocedures;

procedures   : procedures PROCEDURE proc_head IS declarations BEGIN commands END
             | procedures PROCEDURE proc_head IS BEGIN commands END
             | PROCEDURE proc_head IS declarations BEGIN commands END
             | PROCEDURE proc_head IS BEGIN commands END;

main         : PROGRAM IS declarations BEGIN commands END #Declare
             | PROGRAM IS BEGIN commands END #NoDeclare;

commands     : commands command
             | command;

command      : identifier ':=' expression';' #Assign 
             | IF condition THEN commands ENDIF #If
             | IF condition THEN ifblock=commands ELSE elseblock=commands ENDIF #IfElse
             | WHILE condition DO commands ENDWHILE #While 
             | REPEAT commands UNTIL condition';' #Repeat
             | FOR PIDENTIFIER FROM value TO value DO commands ENDFOR #ForUp
             | FOR PIDENTIFIER FROM value DOWNTO value DO commands ENDFOR #ForDown
             | proc_call';' #Call
             | READ identifier';' #Read
             | WRITE value';' #Write;

proc_head    : PIDENTIFIER '(' args_decl ')';

proc_call    : PIDENTIFIER '(' args ')';

declarations : declarations',' PIDENTIFIER
             | declarations',' PIDENTIFIER'['NUM':'NUM']'
             | PIDENTIFIER
             | PIDENTIFIER'['NUM':'NUM']';

args_decl    : args_decl',' PIDENTIFIER
             | args_decl',' T PIDENTIFIER
             | PIDENTIFIER
             | T PIDENTIFIER;

args         : args',' PIDENTIFIER
             | PIDENTIFIER;

expression   : value #Eval
             | left=value '+' right=value #Add
             | left=value '-' right=value #Sub
             | left=value '*' right=value #Mul
             | left=value '/' right=value #Div
             | left=value '%' right=value #Mod;

condition    : left=value '=' right=value #Eq
             | left=value '!=' right=value #Neq
             | left=value '>' right=value #Ge
             | left=value '<' right=value #Le
             | left=value '>=' right=value #Geq
             | left=value '<=' right=value #Leq;

value        : NUM #Num
             | identifier #Id;

identifier   : PIDENTIFIER;
//             | PIDENTIFIER'['PIDENTIFIER']'
//             | PIDENTIFIER'['NUM']';

