namespace BisUtils.EnConfig.Context;

using Core.ParsingFramework.Parser;

public struct EnConfigContext : IBisParserContext
{
    public EnConfigContext()
    {
    }

    public Stack<object> CurrentContext { get; } = new Stack<object>();
    public bool ShouldEnd { get; set; } = false;
}
