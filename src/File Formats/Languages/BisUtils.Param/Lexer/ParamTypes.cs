namespace BisUtils.Param.Lexer;

public enum ParamTypes
{
    EOF = -1,
    Invalid = 0,
    AbsPreprocess = 1,
    AbsString = 2,
    AbsFloat = 3,
    AbsInteger = 4,
    AbsWhitespace = 5,
    KwClass,
    KwEnum,
    KwDelete,
    AbsIdentifier,
    SymColon,
    SymSeparator,
    SymComma,
    SymLSquare,
    SymRSquare,
    SymLCurly,
    SymRCurly,
    OpAssign,
    OpAddAssign,
    OpSubAssign
}
