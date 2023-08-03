grammar EnConfig;

config: node* EOF;

node:
    nodeName=identifier (optionalName=identifier)? (optionalGuid=stringLiteral)? (optionalConfPath=nodeExtension)? nodeValue=literal EOL;
nodeExtension: Colon stringLiteral;

literal:
    objectLiteral
    stringLiteral |
    intLiteral |
    floatLiteral |
    arrayLiteral |
    enumLiteral;
identifier: Identifier;
enumLiteral: Identifier;
objectLiteral: LCurly node* RCurly;
stringLiteral: StringLiteral;
intLiteral: NumericLiteral;
floatLiteral: DecimalLiteral;
arrayLiteral: LCurly literal* RCurly;
WS: [ \t]+ -> skip;
LCurly: '{' EOL;
RCurly: '}';
Colon: ':';
StringLiteral: Quote .*? Quote;
NumericLiteral: Digit+;
DecimalLiteral: NumericLiteral Period NumericLiteral;
Identifier: LetterOrUnderscore IdentifierCharacter*;
EOL: '\r' '\n' | '\n' | '\r';
fragment Period: '.';
fragment Quote: '"';
fragment Digit: [0-9];
fragment Letter: [a-zA-Z];
fragment LetterOrUnderscore: Letter | '_';
fragment IdentifierCharacter: Digit | LetterOrUnderscore;


