namespace BisUtils.RvConfig.Parse;

using LangAssembler.Parser.Models;
using Lexer;
using Models;
using Models.Statements;

public class RvConfigParseContext : IParserContext
{

    public RvConfigLexicalStage LexicalStage { get; set; } = RvConfigLexicalStage.Code;
    public bool ShouldEnd { get; set; }
    public Stack<IParamClass> Context { get; set; }
    public IParamClass CurrentContext => Context.Peek();
    public IRvConfigFile Root { get; set; }
    public bool InArray { get; set; }

    public RvConfigParseContext()
    {
        ShouldEnd = false;
        Root = new RvConfigFile();
        Context = new Stack<IParamClass>();
        Context.Push(Root);
    }

}
