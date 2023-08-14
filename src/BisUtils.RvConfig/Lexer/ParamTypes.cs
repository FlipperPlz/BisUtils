namespace BisUtils.RvConfig.Lexer;

public enum ParamTypes
{
    EOF = -1,
    Invalid = 0,
    AbsPreprocess = 1,
    AbsLiteral = 2,
    AbsWhitespace = 3,
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
