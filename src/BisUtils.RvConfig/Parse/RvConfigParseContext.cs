namespace BisUtils.RvConfig.Parse;

using Core.Parsing.Parser;
using Models;
using Models.Statements;

public class RvConfigParseContext : IBisParserContext
{


    public bool ShouldEnd { get; set; }
    public Stack<IParamClass> Context { get; set; }
    public IParamClass CurrentContext => Context.Peek();
    public IParamFile Root { get; set; }

    public RvConfigParseContext()
    {
        ShouldEnd = false;
        Root = new ParamFile();
        Context = new Stack<IParamClass>();
        Context.Push(Root);
    }

}
