lexer grammar PreProcLexer;

@header {namespace BisUtils.Generated.PreProcessor;}

SHARP: '#'                                   -> mode(DIRECTIVE_MODE);
ENTER_MACRO_MODE: '__'                       -> mode(MACRO_MODE);
SINGLE_LINE_COMMENT: '//' ~[\r\n]*           -> channel(HIDDEN);
EMPTY_DELIMITED_COMMENT: ('/*/' | '/**/')    -> channel(HIDDEN);
DELIMITED_COMMENT: '/*' .*? '*/'             -> channel(HIDDEN);
CONCAT:    '##';
CODE: ~[_#]+;

mode MACRO_MODE;
LN_MACRO: 'LINE';
FL_MACRO: 'FILE';
LEAVE_MACRO_MODE: '__'                      -> mode(DEFAULT_MODE);

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

IDENTIFIER: LETTER (LETTER | [0-9])*;
LITERAL_INT: Number | HEXADECIMAL;
LITERAL_FLOAT: DecimalNumber;
LITERAL_STRING: '"' (. | DIRECITVE_NEW_LINE)*? '"';
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