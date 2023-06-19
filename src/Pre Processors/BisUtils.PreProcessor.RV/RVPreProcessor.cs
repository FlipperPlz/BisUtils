namespace BisUtils.PreProcessor.RV;

using Core.Parsing;
using FResults;
using Lexer;
using Models.Directives;
using Models.Stubs;
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
                    results.Add(ParseDirective(lexer, out var directive));
                    if (directive != null)
                    {
                        results.Add(EvaluateDirective(directive, out var text));
                        lexer.ReplaceRange(start..lexer.Position, text);
                    }
                    continue;
                }
            }

            if (lexer.Position == start)
            {
                lexer.MoveForward();
            }
        }

        return Result.Merge(results);
    }

    private Result EvaluateDirective(IRVDirective directive, out string evaluated)
    {
        switch (directive)
        {
            case IRVIncludeDirective includeDirective:
                return IncludeLocator(includeDirective, out evaluated);
            case IRVDefineDirective macro:
            {
                evaluated = "";
                MacroDefinitions.Add(macro);
                return Result.ImmutableOk();
            }
            case IRVUndefineDirective undefineDirective:
            {
                evaluated = "";

                var macro = FindMacro(undefineDirective.MacroName);
                if (macro is null)
                {
                    return Result.Fail($"Couldn't locate macro named '{undefineDirective.MacroName}'");
                }

                MacroDefinitions.Remove(macro);
                return Result.ImmutableOk();
            }
            case IRVIfNotDefinedDirective ifNotDefinedDirective:
            {

                evaluated = MacroDefinitions.All(e => e.MacroName != ifNotDefinedDirective.MacroName)
                    ? ifNotDefinedDirective.IfBody
                    : ifNotDefinedDirective.ElseBody ?? "";
                return Result.Ok();
            }
            case IRVIfDefinedDirective ifDefinedDirective:
            {
                evaluated = MacroDefinitions.Any(e => e.MacroName == ifDefinedDirective.MacroName)
                    ? ifDefinedDirective.IfBody
                    : ifDefinedDirective.ElseBody ?? "";
                return Result.Ok();
            }
            default:
            {
                evaluated = "";
                return Result.Fail($"Evaluation isn't implemented for this directive type.'");
            }
        }
    }

    public IRVDefineDirective? FindMacro(string name) =>
        MacroDefinitions.FirstOrDefault(e => e.MacroName == name);

    public Result ParseDirective(RVLexer lexer, out IRVDirective? directive)
    {
        var results = new List<Result>();

        switch (lexer.ReadWord())
        {
            case "include":
            {
                results.Add(RVIncludeDirective.ParseDirective(this, lexer, out var include));
                directive = include;
                break;
            }
            case "undefine":
            {
                results.Add(lexer.TraverseWhitespace(out _, false, false, true, false));
                directive = new RVUndefineDirective(this, lexer.ReadWord());
                break;
            }
            case "define":
            {
                results.Add(lexer.TraverseWhitespace(out _, false, false, true, false));
                var macroName = lexer.ReadMacroId(true);
                var macroArguments = lexer.ReadMacroArguments().ToList();
                results.Add(lexer.TraverseWhitespace(out _, false, false, true, false));
                var start = lexer.Position;
                results.Add(lexer.TraverseLine(out var lineLength, true, true));
                var macroValue = lexer.GetRange(start..(start + lineLength));

                directive = new RVDefineDirective(this, macroName, macroValue, macroArguments);
                break;
            }
            default:
                directive = null;
                results.Add(Result.Fail("Invalid Directive"));
                break;
        }

        return Result.Merge(results);
    }
}
