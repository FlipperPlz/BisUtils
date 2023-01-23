parser grammar PreProcParser;

options { tokenVocab=PreProcLexer; }
@header {namespace BisUtils.Generated.PreProcessor;}

preprocessor: text* EOF;

text: code |
    directive |
    lineMacro (text | newLineOrEOF) |
    fileMacro (text | newLineOrEOF) ;
    
lineMacro: ENTER_MACRO_MODE LN_MACRO LEAVE_MACRO_MODE;
fileMacro: ENTER_MACRO_MODE FL_MACRO LEAVE_MACRO_MODE;

directive: 
    SHARP INCLUDE LITERAL_STRING                             newLineOrEOF |
    SHARP DEFINE preprocessor_macro preprocessor_expression? newLineOrEOF |
    SHARP UNDEFINE IDENTIFIER                                newLineOrEOF |
    SHARP ifdefDirective                                     newLineOrEOF |
    SHARP ifndefDirective                                    newLineOrEOF;
    
ifdefDirective: IFDEF IDENTIFIER newLineOrEOF text* (elseDirective | endIfDirective);
ifndefDirective: IFNDEF IDENTIFIER newLineOrEOF text* (elseDirective | endIfDirective);

elseDirective: SHARP ELSE newLineOrEOF text* endIfDirective;
endIfDirective: SHARP ENDIF;

preprocessor_expression: LITERAL_FLOAT                                                          #preprocessorConstant |
                         LITERAL_INT                                                            #preprocessorConstant |
                         preprocessor_macro                                                     #preprocessorMacro; 
preprocessor_macro: IDENTIFIER (LParenthesis preprocessor_expression RParenthesis)?;

newLineOrEOF: (NEW_LINE | EOF);
code: CODE+;