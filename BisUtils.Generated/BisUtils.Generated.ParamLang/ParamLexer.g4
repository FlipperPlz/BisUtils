lexer grammar ParamLexer;

@header {namespace BisUtils.Generated.ParamLang;}

SINGLE_LINE_COMMENT: '//' ~[\r\n]*           -> channel(HIDDEN);
EMPTY_DELIMITED_COMMENT: ('/*/' | '/**/')    -> channel(HIDDEN);
DELIMITED_COMMENT: '/*' .*? '*/'             -> channel(HIDDEN);
WHITESPACES: [\r\n \t]                       -> channel(HIDDEN);

Enum:               'enum';
Class:              'class';
Delete:             'delete';
Add_Assign:         '+=';
Sub_Assign:         '-=';
Assign:             '=';
LSBracket:          '[';
RSBracket:          ']';
LCBracket:          '{';
RCBracket:          '}';
Semicolon:          ';';
Colon:              ':';
Comma:              ',';
DoubleQuote:        '"';
Identifier: [a-zA-Z_] [a-zA-Z_0-9]*;

LiteralString: '"'( ('""'|~('"'))*)'"';
LiteralInteger: Number;
LiteralFloat: DecimalNumber | ScientificNumber;

fragment Number: ('-')? [0-9]+;
fragment DecimalNumber:  Number '.' [0-9]+;
fragment ScientificNumber: AnyNumber ('e'|'E') ('+'|'-') AnyNumber;
fragment AnyNumber: DecimalNumber | Number;