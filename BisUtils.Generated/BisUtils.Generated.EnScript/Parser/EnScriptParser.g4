parser grammar EnScriptParser;

@header {namespace BisUtils.Generated.EnScript;}

options { 
    tokenVocab=EnScriptLexer; 
}

computationalStart: (globalDeclaration | funcOrVar | typeDeclaration)* EOF;

typeDeclaration: TYPEDEF typeReference arrayIndex? identifier Semicolon;

globalDeclaration: attribute? typeModifier* (classDeclaration | enumDeclaration) Semicolon?;

globalInheritance: ((Colon | EXTENDS) typeReference (Comma typeReference)*);

enumDeclaration: ENUM identifier globalInheritance? enumBody;

enumBody: enumMember | LCurly enumMember* Comma* RCurly;

enumMember: identifier (Assign expression)? (Comma enumMember)*;

classDeclaration: CLASS identifier genericTypeDeclarationList? globalInheritance? classBody;

classBody: ( funcVarOrDeconstructor ) | ( LCurly funcVarOrDeconstructor* RCurly );

funcOrVar: attribute? (functionDeclaration | variableDeclaration);

funcVarOrDeconstructor: funcOrVar | deconstructorDeclaration;

variableDeclaration: variableModifier* typeReference variableDeclarator Semicolon;

variableDeclarator: identifier arrayIndex? (definedVariableDeclarator | undefinedVariableDeclarator);

undefinedVariableDeclarator: (Comma undefinedVariableDeclarator)*;

definedVariableDeclarator: (Assign expression) (Comma definedVariableDeclarator)*;

functionDeclaration: functionModifier* typeReference identifier LParenthesis  (functionDeclarationParameters (Comma functionDeclarationParameters)*)? RParenthesis ((functionBody Semicolon?) | Semicolon);

deconstructorDeclaration: functionModifier* typeReference BitwiseNot identifier LParenthesis RParenthesis functionBody Semicolon?;

functionDeclarationParameters: variableModifier* typeReference (functionDeclarationUndefinedParameter | functionDeclarationDefinedParameter);

functionDeclarationDefinedParameter: (identifier arrayIndex? definedVariableDeclarator);

functionDeclarationUndefinedParameter: (identifier arrayIndex? undefinedVariableDeclarator);

functionBody: statement; 

encapsulatedFunctionBody: LCurly statement* RCurly;

functionCall: identifier functionCallParameters;

functionCallParameters: LParenthesis (functionCallParameter )? RParenthesis;

functionCallParameter: ((identifier Colon expression) | (expression)) (Comma functionCallParameter)*;

attribute: LSBracket functionCall RSBracket;

statement: primaryStatement                                                           |
           deleteStatement                                                            |
           threadStatement                                                            |
           gotoStatement                                                              |
           ifStatement                                                                |
           forStatement                                                               |
           foreachStatement                                                           |
           breakStatement                                                             |
           continueStatement                                                          |
           returnStatement                                                            |
           switchStatement                                                            |
           lambdaStatement                                                            |
           whileStatement;

primaryStatement: expression Semicolon                                                |
                  encapsulatedFunctionBody                                            |
                  variableDeclaration                                                 |
                  Semicolon                                                           ;

foreachStatement: FOREACH LParenthesis foreachControl Colon expression RParenthesis functionBody;     

foreachControl: variableModifier* typeReference identifier arrayIndex (Comma foreachControl)?;

forStatement: FOR LParenthesis forControl RParenthesis functionBody;

forControl: statement expression Semicolon expression? Semicolon*;

ifStatement: IF parenthesisedExpression functionBody elseStatement?;

elseStatement: ELSE functionBody;

objectCreation: NEW variableModifier* typeReference functionCallParameters?;

whileStatement: WHILE parenthesisedExpression functionBody;

threadStatement: THREAD functionCall Semicolon;

gotoStatement: GOTO expression Semicolon;

deleteStatement: DELETE expression Semicolon;

lambdaStatement: typeReference identifier functionCallParameters Semicolon;

breakStatement: BREAK Semicolon;

continueStatement: CONTINUE Semicolon;

returnStatement: RETURN expression? Semicolon;

switchStatement: SWITCH parenthesisedExpression LCurly switchLabel* defaultSwitchLabel? switchLabel* RCurly;

switchLabel: CASE expression Colon statement*;
 
defaultSwitchLabel: DEFAULT Colon statement*;

expression:
    THIS | SUPER                                                                      |
    primaryExpression                                                                 |
    parenthesisedExpression                                                           |
    expression op=Dot (identifier | functionCall)                                     |
    expression suffix=(Increment | Decrement)                                         |
    prefix=(Increment | Decrement | Bang | BitwiseNot | Add | Subtract) expression    |
    expression op=(Multiply | Divide | Modulo) expression                             |
    expression op=(Add | Subtract) expression                                         |
    expression Greater Greater expression                                             |
    expression Less Less expression                                                   |
    expression op=(LessEqual | GreaterEqual | Less | Greater) expression              |
    expression op=(Equal | Inequal) expression                                        |
    expression op=(BitwiseOr | BitwiseAnd | BitwiseNot | BitwiseXor) expression       |
    expression op=(LogicalAnd | LogicalOr) expression                                 |
    <assoc=right> expression op=
        (
          Assign           |
          Add_Assign       |
          Subtract_Assign  |
          Multiply_Assign  |
          Divide_Assign    |
          Or_Assign        |
          And_Assign       |
          LShift_Assign    |
          RShift_Assign
        ) expression                                                                  ;    
    
primaryExpression: 
    functionCall            |
    objectCreation          |
    negatedExpression       |
    identifier arrayIndex?  |
    typeReference           |
    literalNull             |
    literalBoolean          |
    literalNumeric          |
    literalString           |
    literalArray;
    
negatedExpression: Bang expression;

parenthesisedExpression: LParenthesis expression RParenthesis;

arrayIndex: LSBracket expression? RSBracket;

literalString: LiteralString;

literalBoolean: LiteralBoolean;

literalNull: NULL;

literalArray: LCurly ( expression (Comma expression)* )? RCurly;

literalNumeric: literalFloat | literalInteger;

literalFloat: LiteralFloat;

literalInteger: LiteralInteger;

identifier: IDENTIFIER | VOID;

genericTypeDeclarationList:  Less genericTypeDeclaration (Comma genericTypeDeclaration)* Greater;

genericTypeDeclaration: variableModifier* typeReference typeName=identifier;

genericTypedReference: Less variableModifier* typeReference (Comma variableModifier* typeReference)* Greater;

typeReference: identifier genericTypedReference?;

typeModifier: MODDED | SEALED;

sharedFuncOrVarModifiers: PRIVATE | PROTECTED | REF | REFERENCE | STATIC | OWNED | PROTO;

variableModifier: AUTOPTR | CONST | OUT | NOTNULL | INOUT | LOCAL | sharedFuncOrVarModifiers;

functionModifier: EXTERNAL | OVERRIDE | NATIVE | VOLATILE | EVENT | sharedFuncOrVarModifiers;