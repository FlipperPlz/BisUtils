parser grammar ParamParser;

@header {namespace BisUtils.Generated.ParamLang;}

options { tokenVocab=ParamLexer; }

computationalStart: ( enumDeclaration | statement )*;

enumDeclaration: Enum LCBracket (enumValue (Comma enumValue)* Comma?)? RCBracket Semicolon;
enumValue: identifier (Assign literalInteger)?;


statement: 
    deleteStatement           Semicolon        |
    arrayAppension            Semicolon        |
    arrayTruncation           Semicolon        |
    arrayDeclaration          Semicolon        |
    tokenDeclaration          Semicolon        |
    classDeclaration          Semicolon        |
    externalClassDeclaration  Semicolon        ;

arrayAppension: arrayName Add_Assign literalArray;
arrayTruncation: arrayName Sub_Assign literalArray;
deleteStatement: Delete identifier;

externalClassDeclaration: Class classname=identifier;
classDeclaration: Class classname=identifier (Colon superclass=identifier)? LCBracket statement* RCBracket;
arrayDeclaration: arrayName Assign value=literalArray;
tokenDeclaration: tokenName=identifier Assign value=literal;

literalArray: LCBracket (literalOrArray (Comma literalOrArray)* Comma?)? RCBracket;
literalString: LiteralString;
literalInteger: LiteralInteger;
literalFloat: LiteralFloat;

literalOrArray: literal | literalArray;
literal: literalString | literalInteger | literalFloat;

arrayName: identifier LSBracket RSBracket;
identifier: Identifier;