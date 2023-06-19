namespace BisUtils.PreProcessor.RV;

using Core.Parsing;
using FResults;
using Lexer;
using Models.Directives;
using Utils;

public interface IRVPreProcessor : IBisPreProcessor<RVLexer>
{
    List<IRVDefineDirective> MacroDefinitions { get; }

    RVIncludeFinder IncludeLocator { get; }
}

public class RVPreProcessor : IRVPreProcessor
{
    public List<IRVDefineDirective> MacroDefinitions { get; }
    public RVIncludeFinder IncludeLocator { get; }

    public RVPreProcessor(List<IRVDefineDirective> macroDefinitions, RVIncludeFinder includeLocator)
    {
        MacroDefinitions = macroDefinitions;
        IncludeLocator = includeLocator;
    }

    public Result ProcessLexer(RVLexer lexer)
    {
        var quoted = false;
        var results = new List<Result>();
        while (!lexer.IsEOF())
        {
            var start = lexer.Position;
            if (lexer.CurrentChar == '"')
            {
                quoted = !quoted;
            }

            if (quoted)
            {
                lexer.MoveForward();
                continue;
            }

            switch (lexer.CurrentChar)
            {
                case '/':
                {
                    results.Add(lexer.TraverseComment(out _, out _, true));
                    continue;
                }
                case '#':
                {
                    switch (lexer.ReadWord())
                    {
                        case "include":
                        {
                            results.Add(RVIncludeDirective.ParseDirective(this, lexer, out var include));
                            //include//.Process()
                            break;
                        }
                        case "undefine":
                        {
                            lexer.TraverseWhitespace(out _, false, false, true, false);
                            var undefine = new RVUndefineDirective(this, lexer.ReadWord());
                            break;
                        }
                    }

                    //Replace Directive
                    continue;
                }
            }
        }

        return Result.Merge(results);
    }
}
