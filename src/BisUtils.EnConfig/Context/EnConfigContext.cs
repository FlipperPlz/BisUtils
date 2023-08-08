namespace BisUtils.EnConfig.Context;

using Core.Parsing.Parser;

public struct EnConfigContext : IBisParserContext
{
    public EnConfigContext()
    {
    }

    public Stack<object> CurrentContext { get; } = new Stack<object>();
    public bool ShouldEnd { get; } = false;
}
