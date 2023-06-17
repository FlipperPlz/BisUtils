namespace BisUtils.Core.Parsing;

public abstract class BisLexer : BisStringStepper
{
    protected BisLexer(string content) : base(content)
    {
    }
}
