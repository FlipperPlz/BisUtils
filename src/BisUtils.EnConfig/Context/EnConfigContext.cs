namespace BisUtils.EnConfig.Context;

using LangAssembler.Parser.Models;

public struct EnConfigContext : IParserContext
{
    public EnConfigContext()
    {
    }

    public Stack<object> CurrentContext { get; } = new Stack<object>();
    public bool ShouldEnd { get; set; } = false;
}
