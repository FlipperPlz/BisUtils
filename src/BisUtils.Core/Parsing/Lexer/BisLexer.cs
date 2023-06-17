namespace BisUtils.Core.Parsing.Lexer;

public abstract class BisLexer : BisStringStepper
{
    protected BisLexer(string content) : base(content)
    {
    }
}
