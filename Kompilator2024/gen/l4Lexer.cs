//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:/Users/Rafa�/Sem5/Jftt/Kompilator/Kompilator2024/Kompilator2024/l4.g4 by ANTLR 4.13.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.2")]
[System.CLSCompliant(false)]
public partial class l4Lexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, T__10=11, T__11=12, T__12=13, T__13=14, T__14=15, T__15=16, T__16=17, 
		T__17=18, T__18=19, COMMENT=20, WS=21, WHITESPACE=22, PIDENTIFIER=23, 
		NUM=24, PROCEDURE=25, IS=26, BEGIN=27, END=28, PROGRAM=29, IF=30, WHILE=31, 
		FOR=32, REPEAT=33, THEN=34, ELSE=35, ENDIF=36, DO=37, ENDWHILE=38, UNTIL=39, 
		FROM=40, ENDFOR=41, TO=42, DOWNTO=43, READ=44, WRITE=45, T=46;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"T__0", "T__1", "T__2", "T__3", "T__4", "T__5", "T__6", "T__7", "T__8", 
		"T__9", "T__10", "T__11", "T__12", "T__13", "T__14", "T__15", "T__16", 
		"T__17", "T__18", "COMMENT", "WS", "WHITESPACE", "PIDENTIFIER", "NUM", 
		"PROCEDURE", "IS", "BEGIN", "END", "PROGRAM", "IF", "WHILE", "FOR", "REPEAT", 
		"THEN", "ELSE", "ENDIF", "DO", "ENDWHILE", "UNTIL", "FROM", "ENDFOR", 
		"TO", "DOWNTO", "READ", "WRITE", "T"
	};


	public l4Lexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public l4Lexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "':='", "';'", "'('", "')'", "','", "'['", "':'", "']'", "'+'", 
		"'-'", "'*'", "'/'", "'%'", "'='", "'!='", "'>'", "'<'", "'>='", "'<='", 
		null, null, null, null, null, "'PROCEDURE'", "'IS'", "'BEGIN'", "'END'", 
		"'PROGRAM'", "'IF'", "'WHILE'", "'FOR'", "'REPEAT'", "'THEN'", "'ELSE'", 
		"'ENDIF'", "'DO'", "'ENDWHILE'", "'UNTIL'", "'FROM'", "'ENDFOR'", "'TO'", 
		"'DOWNTO'", "'READ'", "'WRITE'", "'T'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, "COMMENT", "WS", "WHITESPACE", 
		"PIDENTIFIER", "NUM", "PROCEDURE", "IS", "BEGIN", "END", "PROGRAM", "IF", 
		"WHILE", "FOR", "REPEAT", "THEN", "ELSE", "ENDIF", "DO", "ENDWHILE", "UNTIL", 
		"FROM", "ENDFOR", "TO", "DOWNTO", "READ", "WRITE", "T"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "l4.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static l4Lexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static int[] _serializedATN = {
		4,0,46,286,6,-1,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,
		6,2,7,7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,2,12,7,12,2,13,7,13,2,14,
		7,14,2,15,7,15,2,16,7,16,2,17,7,17,2,18,7,18,2,19,7,19,2,20,7,20,2,21,
		7,21,2,22,7,22,2,23,7,23,2,24,7,24,2,25,7,25,2,26,7,26,2,27,7,27,2,28,
		7,28,2,29,7,29,2,30,7,30,2,31,7,31,2,32,7,32,2,33,7,33,2,34,7,34,2,35,
		7,35,2,36,7,36,2,37,7,37,2,38,7,38,2,39,7,39,2,40,7,40,2,41,7,41,2,42,
		7,42,2,43,7,43,2,44,7,44,2,45,7,45,1,0,1,0,1,0,1,1,1,1,1,2,1,2,1,3,1,3,
		1,4,1,4,1,5,1,5,1,6,1,6,1,7,1,7,1,8,1,8,1,9,1,9,1,10,1,10,1,11,1,11,1,
		12,1,12,1,13,1,13,1,14,1,14,1,14,1,15,1,15,1,16,1,16,1,17,1,17,1,17,1,
		18,1,18,1,18,1,19,1,19,5,19,138,8,19,10,19,12,19,141,9,19,1,19,1,19,1,
		20,4,20,146,8,20,11,20,12,20,147,1,20,1,20,1,21,4,21,153,8,21,11,21,12,
		21,154,1,22,4,22,158,8,22,11,22,12,22,159,1,23,4,23,163,8,23,11,23,12,
		23,164,1,24,1,24,1,24,1,24,1,24,1,24,1,24,1,24,1,24,1,24,1,25,1,25,1,25,
		1,26,1,26,1,26,1,26,1,26,1,26,1,27,1,27,1,27,1,27,1,28,1,28,1,28,1,28,
		1,28,1,28,1,28,1,28,1,29,1,29,1,29,1,30,1,30,1,30,1,30,1,30,1,30,1,31,
		1,31,1,31,1,31,1,32,1,32,1,32,1,32,1,32,1,32,1,32,1,33,1,33,1,33,1,33,
		1,33,1,34,1,34,1,34,1,34,1,34,1,35,1,35,1,35,1,35,1,35,1,35,1,36,1,36,
		1,36,1,37,1,37,1,37,1,37,1,37,1,37,1,37,1,37,1,37,1,38,1,38,1,38,1,38,
		1,38,1,38,1,39,1,39,1,39,1,39,1,39,1,40,1,40,1,40,1,40,1,40,1,40,1,40,
		1,41,1,41,1,41,1,42,1,42,1,42,1,42,1,42,1,42,1,42,1,43,1,43,1,43,1,43,
		1,43,1,44,1,44,1,44,1,44,1,44,1,44,1,45,1,45,0,0,46,1,1,3,2,5,3,7,4,9,
		5,11,6,13,7,15,8,17,9,19,10,21,11,23,12,25,13,27,14,29,15,31,16,33,17,
		35,18,37,19,39,20,41,21,43,22,45,23,47,24,49,25,51,26,53,27,55,28,57,29,
		59,30,61,31,63,32,65,33,67,34,69,35,71,36,73,37,75,38,77,39,79,40,81,41,
		83,42,85,43,87,44,89,45,91,46,1,0,5,2,0,10,10,13,13,3,0,9,10,13,13,32,
		32,3,0,9,10,12,13,32,32,2,0,95,95,97,122,1,0,48,57,290,0,1,1,0,0,0,0,3,
		1,0,0,0,0,5,1,0,0,0,0,7,1,0,0,0,0,9,1,0,0,0,0,11,1,0,0,0,0,13,1,0,0,0,
		0,15,1,0,0,0,0,17,1,0,0,0,0,19,1,0,0,0,0,21,1,0,0,0,0,23,1,0,0,0,0,25,
		1,0,0,0,0,27,1,0,0,0,0,29,1,0,0,0,0,31,1,0,0,0,0,33,1,0,0,0,0,35,1,0,0,
		0,0,37,1,0,0,0,0,39,1,0,0,0,0,41,1,0,0,0,0,43,1,0,0,0,0,45,1,0,0,0,0,47,
		1,0,0,0,0,49,1,0,0,0,0,51,1,0,0,0,0,53,1,0,0,0,0,55,1,0,0,0,0,57,1,0,0,
		0,0,59,1,0,0,0,0,61,1,0,0,0,0,63,1,0,0,0,0,65,1,0,0,0,0,67,1,0,0,0,0,69,
		1,0,0,0,0,71,1,0,0,0,0,73,1,0,0,0,0,75,1,0,0,0,0,77,1,0,0,0,0,79,1,0,0,
		0,0,81,1,0,0,0,0,83,1,0,0,0,0,85,1,0,0,0,0,87,1,0,0,0,0,89,1,0,0,0,0,91,
		1,0,0,0,1,93,1,0,0,0,3,96,1,0,0,0,5,98,1,0,0,0,7,100,1,0,0,0,9,102,1,0,
		0,0,11,104,1,0,0,0,13,106,1,0,0,0,15,108,1,0,0,0,17,110,1,0,0,0,19,112,
		1,0,0,0,21,114,1,0,0,0,23,116,1,0,0,0,25,118,1,0,0,0,27,120,1,0,0,0,29,
		122,1,0,0,0,31,125,1,0,0,0,33,127,1,0,0,0,35,129,1,0,0,0,37,132,1,0,0,
		0,39,135,1,0,0,0,41,145,1,0,0,0,43,152,1,0,0,0,45,157,1,0,0,0,47,162,1,
		0,0,0,49,166,1,0,0,0,51,176,1,0,0,0,53,179,1,0,0,0,55,185,1,0,0,0,57,189,
		1,0,0,0,59,197,1,0,0,0,61,200,1,0,0,0,63,206,1,0,0,0,65,210,1,0,0,0,67,
		217,1,0,0,0,69,222,1,0,0,0,71,227,1,0,0,0,73,233,1,0,0,0,75,236,1,0,0,
		0,77,245,1,0,0,0,79,251,1,0,0,0,81,256,1,0,0,0,83,263,1,0,0,0,85,266,1,
		0,0,0,87,273,1,0,0,0,89,278,1,0,0,0,91,284,1,0,0,0,93,94,5,58,0,0,94,95,
		5,61,0,0,95,2,1,0,0,0,96,97,5,59,0,0,97,4,1,0,0,0,98,99,5,40,0,0,99,6,
		1,0,0,0,100,101,5,41,0,0,101,8,1,0,0,0,102,103,5,44,0,0,103,10,1,0,0,0,
		104,105,5,91,0,0,105,12,1,0,0,0,106,107,5,58,0,0,107,14,1,0,0,0,108,109,
		5,93,0,0,109,16,1,0,0,0,110,111,5,43,0,0,111,18,1,0,0,0,112,113,5,45,0,
		0,113,20,1,0,0,0,114,115,5,42,0,0,115,22,1,0,0,0,116,117,5,47,0,0,117,
		24,1,0,0,0,118,119,5,37,0,0,119,26,1,0,0,0,120,121,5,61,0,0,121,28,1,0,
		0,0,122,123,5,33,0,0,123,124,5,61,0,0,124,30,1,0,0,0,125,126,5,62,0,0,
		126,32,1,0,0,0,127,128,5,60,0,0,128,34,1,0,0,0,129,130,5,62,0,0,130,131,
		5,61,0,0,131,36,1,0,0,0,132,133,5,60,0,0,133,134,5,61,0,0,134,38,1,0,0,
		0,135,139,5,35,0,0,136,138,8,0,0,0,137,136,1,0,0,0,138,141,1,0,0,0,139,
		137,1,0,0,0,139,140,1,0,0,0,140,142,1,0,0,0,141,139,1,0,0,0,142,143,6,
		19,0,0,143,40,1,0,0,0,144,146,7,1,0,0,145,144,1,0,0,0,146,147,1,0,0,0,
		147,145,1,0,0,0,147,148,1,0,0,0,148,149,1,0,0,0,149,150,6,20,0,0,150,42,
		1,0,0,0,151,153,7,2,0,0,152,151,1,0,0,0,153,154,1,0,0,0,154,152,1,0,0,
		0,154,155,1,0,0,0,155,44,1,0,0,0,156,158,7,3,0,0,157,156,1,0,0,0,158,159,
		1,0,0,0,159,157,1,0,0,0,159,160,1,0,0,0,160,46,1,0,0,0,161,163,7,4,0,0,
		162,161,1,0,0,0,163,164,1,0,0,0,164,162,1,0,0,0,164,165,1,0,0,0,165,48,
		1,0,0,0,166,167,5,80,0,0,167,168,5,82,0,0,168,169,5,79,0,0,169,170,5,67,
		0,0,170,171,5,69,0,0,171,172,5,68,0,0,172,173,5,85,0,0,173,174,5,82,0,
		0,174,175,5,69,0,0,175,50,1,0,0,0,176,177,5,73,0,0,177,178,5,83,0,0,178,
		52,1,0,0,0,179,180,5,66,0,0,180,181,5,69,0,0,181,182,5,71,0,0,182,183,
		5,73,0,0,183,184,5,78,0,0,184,54,1,0,0,0,185,186,5,69,0,0,186,187,5,78,
		0,0,187,188,5,68,0,0,188,56,1,0,0,0,189,190,5,80,0,0,190,191,5,82,0,0,
		191,192,5,79,0,0,192,193,5,71,0,0,193,194,5,82,0,0,194,195,5,65,0,0,195,
		196,5,77,0,0,196,58,1,0,0,0,197,198,5,73,0,0,198,199,5,70,0,0,199,60,1,
		0,0,0,200,201,5,87,0,0,201,202,5,72,0,0,202,203,5,73,0,0,203,204,5,76,
		0,0,204,205,5,69,0,0,205,62,1,0,0,0,206,207,5,70,0,0,207,208,5,79,0,0,
		208,209,5,82,0,0,209,64,1,0,0,0,210,211,5,82,0,0,211,212,5,69,0,0,212,
		213,5,80,0,0,213,214,5,69,0,0,214,215,5,65,0,0,215,216,5,84,0,0,216,66,
		1,0,0,0,217,218,5,84,0,0,218,219,5,72,0,0,219,220,5,69,0,0,220,221,5,78,
		0,0,221,68,1,0,0,0,222,223,5,69,0,0,223,224,5,76,0,0,224,225,5,83,0,0,
		225,226,5,69,0,0,226,70,1,0,0,0,227,228,5,69,0,0,228,229,5,78,0,0,229,
		230,5,68,0,0,230,231,5,73,0,0,231,232,5,70,0,0,232,72,1,0,0,0,233,234,
		5,68,0,0,234,235,5,79,0,0,235,74,1,0,0,0,236,237,5,69,0,0,237,238,5,78,
		0,0,238,239,5,68,0,0,239,240,5,87,0,0,240,241,5,72,0,0,241,242,5,73,0,
		0,242,243,5,76,0,0,243,244,5,69,0,0,244,76,1,0,0,0,245,246,5,85,0,0,246,
		247,5,78,0,0,247,248,5,84,0,0,248,249,5,73,0,0,249,250,5,76,0,0,250,78,
		1,0,0,0,251,252,5,70,0,0,252,253,5,82,0,0,253,254,5,79,0,0,254,255,5,77,
		0,0,255,80,1,0,0,0,256,257,5,69,0,0,257,258,5,78,0,0,258,259,5,68,0,0,
		259,260,5,70,0,0,260,261,5,79,0,0,261,262,5,82,0,0,262,82,1,0,0,0,263,
		264,5,84,0,0,264,265,5,79,0,0,265,84,1,0,0,0,266,267,5,68,0,0,267,268,
		5,79,0,0,268,269,5,87,0,0,269,270,5,78,0,0,270,271,5,84,0,0,271,272,5,
		79,0,0,272,86,1,0,0,0,273,274,5,82,0,0,274,275,5,69,0,0,275,276,5,65,0,
		0,276,277,5,68,0,0,277,88,1,0,0,0,278,279,5,87,0,0,279,280,5,82,0,0,280,
		281,5,73,0,0,281,282,5,84,0,0,282,283,5,69,0,0,283,90,1,0,0,0,284,285,
		5,84,0,0,285,92,1,0,0,0,6,0,139,147,154,159,164,1,6,0,0
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
