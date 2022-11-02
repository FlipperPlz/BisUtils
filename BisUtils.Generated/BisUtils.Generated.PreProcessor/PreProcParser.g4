parser grammar PreProcParser;

options { tokenVocab=PreProcLexer; }

preprocessor: text* EOF;

text: code |
    directive |
    lineMacro text|
    fileMacro;
    
lineMacro: ENTER_MACRO_MODE LN_MACRO LEAVE_MACRO_MODE;
fileMacro: ENTER_MACRO_MODE FL_MACRO LEAVE_MACRO_MODE;


directive: 
    SHARP INCLUDE LITERAL_STRING newLineOrEOF |
    SHARP DEFINE preprocessor_macro preprocessor_expression? newLineOrEOF |
    SHARP UNDEFINE IDENTIFIER newLineOrEOF |
    ifdefDirective |
    ifndefDirective;
    
ifdefDirective: SHARP IFDEF IDENTIFIER newLineOrEOF text* (elseDirective | endIfDirective);
ifndefDirective: SHARP IFNDEF IDENTIFIER newLineOrEOF text* (elseDirective | endIfDirective);

elseDirective: SHARP ELSE newLineOrEOF text* endIfDirective;
endIfDirective: SHARP ENDIF newLineOrEOF;

preprocessor_expression: LITERAL_FLOAT                                                          #preprocessorConstant |
                         LITERAL_INT                                                            #preprocessorConstant |
                         preprocessor_macro                                                     #preprocessorMacro; 
preprocessor_macro: IDENTIFIER (LParenthesis preprocessor_expression RParenthesis)?;

newLineOrEOF: (NEW_LINE | EOF);
code: CODE+;