grammar EnConfig;

config: node* EOF;

node:
    nodeName=identifier (optionalName=identifier)? (optionalId=stringLiteral)? nodeValue    ;

nodeValue: literal | objectBody;
literal:
    stringLiteral |
    intLiteral |
    floatLiteral;
identifier: Identifier;
objectBody: LCurly node* RCurly;
stringLiteral: StringLiteral;
intLiteral: NumericLiteral;
floatLiteral: DecimalLiteral;

LCurly: '{';
RCurly: '}';
Colon: ':';
StringLiteral: Quote .*? Quote;
NumericLiteral: Digit+;
DecimalLiteral: NumericLiteral Period NumericLiteral;
Identifier: LetterOrUnderscore IdentifierCharacter*;

fragment Period: '.';
fragment Quote: '"';
fragment Digit: '0'..'9';
fragment Letter: 'a'..'z' | 'A'..'Z';
fragment LetterOrUnderscore: Letter | '_';
fragment IdentifierCharacter: Digit | LetterOrUnderscore;


