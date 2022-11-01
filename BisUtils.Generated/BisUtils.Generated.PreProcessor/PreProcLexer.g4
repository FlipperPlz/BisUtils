lexer grammar PreProcLexer;

SHARP:     '#' -> mode(DIRECTIVE_MODE);
CONCAT:    '##';
CODE: ~[#]+;

mode DIRECTIVE_MODE;

DIRECTIVE_WHITESPACES:  [ \t]+ -> channel(HIDDEN);

LSBracket:     '[';
RSBracket:     ']';
LParenthesis:  '(';
RParenthesis:  ')';
Comma:         ',';

UNDEFINE:  'undef';
DEFINE:    'define';
INCLUDE:   'include';
IF:        'if';
IFDEF:     'ifdef';
IFNDEF:    'ifndef';
ELSE:      'else';
ENDIF:     'endif';
LINE:      '__LINE__';
FILE:      '__FILE__';

IDENTIFIER: LETTER (LETTER | [0-9])*;
LITERAL_INT: Number | HEXADECIMAL;
LITERAL_FLOAT: DecimalNumber;
NEW_LINE: '\r'? '\n'                       -> mode(DEFAULT_MODE);
DIRECITVE_NEW_LINE: '\\' '\r'? '\n'        -> channel(HIDDEN);

fragment HEXADECIMAL : '0x' ([a-fA-F0-9])+;
fragment Number: '-'? Diget+;
fragment Diget: [0-9];
fragment DecimalNumber:  Number '.' Diget+;

fragment LETTER
    : [$A-Za-z_]
    | ~[\u0000-\u00FF\uD800-\uDBFF]
    | [\uD800-\uDBFF] [\uDC00-\uDFFF]
    | [\u00E9]
    ;

fragment A: [aA];
fragment B: [bB];
fragment C: [cC];
fragment D: [dD];
fragment E: [eE];
fragment F: [fF];
fragment G: [gG];
fragment H: [hH];
fragment I: [iI];
fragment J: [jJ];
fragment K: [kK];
fragment L: [lL];
fragment M: [mM];
fragment N: [nN];
fragment O: [oO];
fragment P: [pP];
fragment Q: [qQ];
fragment R: [rR];
fragment S: [sS];
fragment T: [tT];
fragment U: [uU];
fragment V: [vV];
fragment W: [wW];
fragment X: [xX];
fragment Y: [yY];
fragment Z: [zZ];