//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.10.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from /Users/ryannkelly/Desktop/BisUtils/BisUtils.Generated/BisUtils.Generated.ParamLang/ParamLexer.g4 by ANTLR 4.10.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace BisUtils.Generated.ParamLang;
using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.10.1")]
[System.CLSCompliant(false)]
public partial class ParamLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		SINGLE_LINE_COMMENT=1, EMPTY_DELIMITED_COMMENT=2, DELIMITED_COMMENT=3, 
		WHITESPACES=4, Enum=5, Class=6, Delete=7, Add_Assign=8, Sub_Assign=9, 
		Assign=10, LSBracket=11, RSBracket=12, LCBracket=13, RCBracket=14, Semicolon=15, 
		Colon=16, Comma=17, DoubleQuote=18, Identifier=19, LiteralString=20, LiteralInteger=21, 
		LiteralFloat=22;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"SINGLE_LINE_COMMENT", "EMPTY_DELIMITED_COMMENT", "DELIMITED_COMMENT", 
		"WHITESPACES", "Enum", "Class", "Delete", "Add_Assign", "Sub_Assign", 
		"Assign", "LSBracket", "RSBracket", "LCBracket", "RCBracket", "Semicolon", 
		"Colon", "Comma", "DoubleQuote", "Identifier", "LiteralString", "LiteralInteger", 
		"LiteralFloat", "Number", "DecimalNumber", "ScientificNumber"
	};


	public ParamLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public ParamLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, null, null, null, null, "'enum'", "'class'", "'delete'", "'+='", 
		"'-='", "'='", "'['", "']'", "'{'", "'}'", "';'", "':'", "','", "'\"'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "SINGLE_LINE_COMMENT", "EMPTY_DELIMITED_COMMENT", "DELIMITED_COMMENT", 
		"WHITESPACES", "Enum", "Class", "Delete", "Add_Assign", "Sub_Assign", 
		"Assign", "LSBracket", "RSBracket", "LCBracket", "RCBracket", "Semicolon", 
		"Colon", "Comma", "DoubleQuote", "Identifier", "LiteralString", "LiteralInteger", 
		"LiteralFloat"
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

	public override string GrammarFileName { get { return "ParamLexer.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static ParamLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static int[] _serializedATN = {
		4,0,22,177,6,-1,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,
		6,2,7,7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,2,12,7,12,2,13,7,13,2,14,
		7,14,2,15,7,15,2,16,7,16,2,17,7,17,2,18,7,18,2,19,7,19,2,20,7,20,2,21,
		7,21,2,22,7,22,2,23,7,23,2,24,7,24,1,0,1,0,1,0,1,0,5,0,56,8,0,10,0,12,
		0,59,9,0,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,3,1,70,8,1,1,1,1,1,1,2,1,
		2,1,2,1,2,5,2,78,8,2,10,2,12,2,81,9,2,1,2,1,2,1,2,1,2,1,2,1,3,1,3,1,3,
		1,3,1,4,1,4,1,4,1,4,1,4,1,5,1,5,1,5,1,5,1,5,1,5,1,6,1,6,1,6,1,6,1,6,1,
		6,1,6,1,7,1,7,1,7,1,8,1,8,1,8,1,9,1,9,1,10,1,10,1,11,1,11,1,12,1,12,1,
		13,1,13,1,14,1,14,1,15,1,15,1,16,1,16,1,17,1,17,1,18,1,18,5,18,136,8,18,
		10,18,12,18,139,9,18,1,19,1,19,1,19,1,19,5,19,145,8,19,10,19,12,19,148,
		9,19,1,19,1,19,1,20,1,20,1,21,1,21,3,21,156,8,21,1,22,3,22,159,8,22,1,
		22,4,22,162,8,22,11,22,12,22,163,1,23,1,23,1,23,4,23,169,8,23,11,23,12,
		23,170,1,24,1,24,1,24,1,24,1,24,1,79,0,25,1,1,3,2,5,3,7,4,9,5,11,6,13,
		7,15,8,17,9,19,10,21,11,23,12,25,13,27,14,29,15,31,16,33,17,35,18,37,19,
		39,20,41,21,43,22,45,0,47,0,49,0,1,0,8,2,0,10,10,13,13,3,0,9,10,13,13,
		32,32,3,0,65,90,95,95,97,122,4,0,48,57,65,90,95,95,97,122,1,0,34,34,1,
		0,48,57,2,0,69,69,101,101,2,0,43,43,45,45,183,0,1,1,0,0,0,0,3,1,0,0,0,
		0,5,1,0,0,0,0,7,1,0,0,0,0,9,1,0,0,0,0,11,1,0,0,0,0,13,1,0,0,0,0,15,1,0,
		0,0,0,17,1,0,0,0,0,19,1,0,0,0,0,21,1,0,0,0,0,23,1,0,0,0,0,25,1,0,0,0,0,
		27,1,0,0,0,0,29,1,0,0,0,0,31,1,0,0,0,0,33,1,0,0,0,0,35,1,0,0,0,0,37,1,
		0,0,0,0,39,1,0,0,0,0,41,1,0,0,0,0,43,1,0,0,0,1,51,1,0,0,0,3,69,1,0,0,0,
		5,73,1,0,0,0,7,87,1,0,0,0,9,91,1,0,0,0,11,96,1,0,0,0,13,102,1,0,0,0,15,
		109,1,0,0,0,17,112,1,0,0,0,19,115,1,0,0,0,21,117,1,0,0,0,23,119,1,0,0,
		0,25,121,1,0,0,0,27,123,1,0,0,0,29,125,1,0,0,0,31,127,1,0,0,0,33,129,1,
		0,0,0,35,131,1,0,0,0,37,133,1,0,0,0,39,140,1,0,0,0,41,151,1,0,0,0,43,155,
		1,0,0,0,45,158,1,0,0,0,47,165,1,0,0,0,49,172,1,0,0,0,51,52,5,47,0,0,52,
		53,5,47,0,0,53,57,1,0,0,0,54,56,8,0,0,0,55,54,1,0,0,0,56,59,1,0,0,0,57,
		55,1,0,0,0,57,58,1,0,0,0,58,60,1,0,0,0,59,57,1,0,0,0,60,61,6,0,0,0,61,
		2,1,0,0,0,62,63,5,47,0,0,63,64,5,42,0,0,64,70,5,47,0,0,65,66,5,47,0,0,
		66,67,5,42,0,0,67,68,5,42,0,0,68,70,5,47,0,0,69,62,1,0,0,0,69,65,1,0,0,
		0,70,71,1,0,0,0,71,72,6,1,0,0,72,4,1,0,0,0,73,74,5,47,0,0,74,75,5,42,0,
		0,75,79,1,0,0,0,76,78,9,0,0,0,77,76,1,0,0,0,78,81,1,0,0,0,79,80,1,0,0,
		0,79,77,1,0,0,0,80,82,1,0,0,0,81,79,1,0,0,0,82,83,5,42,0,0,83,84,5,47,
		0,0,84,85,1,0,0,0,85,86,6,2,0,0,86,6,1,0,0,0,87,88,7,1,0,0,88,89,1,0,0,
		0,89,90,6,3,0,0,90,8,1,0,0,0,91,92,5,101,0,0,92,93,5,110,0,0,93,94,5,117,
		0,0,94,95,5,109,0,0,95,10,1,0,0,0,96,97,5,99,0,0,97,98,5,108,0,0,98,99,
		5,97,0,0,99,100,5,115,0,0,100,101,5,115,0,0,101,12,1,0,0,0,102,103,5,100,
		0,0,103,104,5,101,0,0,104,105,5,108,0,0,105,106,5,101,0,0,106,107,5,116,
		0,0,107,108,5,101,0,0,108,14,1,0,0,0,109,110,5,43,0,0,110,111,5,61,0,0,
		111,16,1,0,0,0,112,113,5,45,0,0,113,114,5,61,0,0,114,18,1,0,0,0,115,116,
		5,61,0,0,116,20,1,0,0,0,117,118,5,91,0,0,118,22,1,0,0,0,119,120,5,93,0,
		0,120,24,1,0,0,0,121,122,5,123,0,0,122,26,1,0,0,0,123,124,5,125,0,0,124,
		28,1,0,0,0,125,126,5,59,0,0,126,30,1,0,0,0,127,128,5,58,0,0,128,32,1,0,
		0,0,129,130,5,44,0,0,130,34,1,0,0,0,131,132,5,34,0,0,132,36,1,0,0,0,133,
		137,7,2,0,0,134,136,7,3,0,0,135,134,1,0,0,0,136,139,1,0,0,0,137,135,1,
		0,0,0,137,138,1,0,0,0,138,38,1,0,0,0,139,137,1,0,0,0,140,146,5,34,0,0,
		141,142,5,34,0,0,142,145,5,34,0,0,143,145,8,4,0,0,144,141,1,0,0,0,144,
		143,1,0,0,0,145,148,1,0,0,0,146,144,1,0,0,0,146,147,1,0,0,0,147,149,1,
		0,0,0,148,146,1,0,0,0,149,150,5,34,0,0,150,40,1,0,0,0,151,152,3,45,22,
		0,152,42,1,0,0,0,153,156,3,47,23,0,154,156,3,49,24,0,155,153,1,0,0,0,155,
		154,1,0,0,0,156,44,1,0,0,0,157,159,5,45,0,0,158,157,1,0,0,0,158,159,1,
		0,0,0,159,161,1,0,0,0,160,162,7,5,0,0,161,160,1,0,0,0,162,163,1,0,0,0,
		163,161,1,0,0,0,163,164,1,0,0,0,164,46,1,0,0,0,165,166,3,45,22,0,166,168,
		5,46,0,0,167,169,7,5,0,0,168,167,1,0,0,0,169,170,1,0,0,0,170,168,1,0,0,
		0,170,171,1,0,0,0,171,48,1,0,0,0,172,173,3,47,23,0,173,174,7,6,0,0,174,
		175,7,7,0,0,175,176,3,47,23,0,176,50,1,0,0,0,11,0,57,69,79,137,144,146,
		155,158,163,170,1,0,1,0
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
