namespace BisUtils.Core.Parsing;

using System.Text;
using FResults;
using Lexer;

public interface IBisPreProcessorBase
{
    public Result EvaluateLexer(BisMutableStringStepper lexer, StringBuilder? builder);

}

public abstract class BisPreProcessorBase : IBisPreProcessorBase
{
    protected BisPreProcessorBase()
    {

    }

    public abstract Result EvaluateLexer(BisMutableStringStepper lexer, StringBuilder? builder);
}


public abstract class BisPreProcessor<TPreProcTypes> : BisPreProcessorBase, IBisLexer<TPreProcTypes> where TPreProcTypes : Enum
{
    public event IBisLexer<TPreProcTypes>.TokenMatched? OnTokenMatched;
    public abstract IEnumerable<IBisLexer<TPreProcTypes>.TokenDefinition> TokenTypes { get; }
    public abstract IBisLexer<TPreProcTypes>.TokenDefinition EOFToken { get; }
    public IBisLexer<TPreProcTypes>.TokenDefinition? ErrorToken => null;
    public IEnumerable<IBisLexer<TPreProcTypes>.TokenMatch> PreviousMatches => previousMatches;
    private readonly List<IBisLexer<TPreProcTypes>.TokenMatch> previousMatches = new();
    public IBisLexer<TPreProcTypes>.TokenMatch? PreviousMatch() => previousMatches.LastOrDefault();
    public IBisLexer<TPreProcTypes>.TokenMatch NextToken(BisMutableStringStepper lexer)
    {
        var value = GetNextToken(lexer);
        OnTokenMatchedHandler(value, this);
        return value;
    }

    protected abstract IBisLexer<TPreProcTypes>.TokenMatch GetNextToken(BisMutableStringStepper lexer);

    public void TokenizeUntilEnd<T>(BisLexer<T> lexer) where T : Enum
    {
        while (NextToken(lexer) != EOFToken.TokenId)
        {
        }
    }

    public IBisLexer<TPreProcTypes>.TokenMatch TokenizeUntil<TTypes>(
        BisLexer<TTypes> lexer,
        Func<IBisLexer<TPreProcTypes>.TokenMatch, bool> until, IBisLexer<TPreProcTypes>.TokenDefinition asToken) where TTypes : Enum
    {
        var start = lexer.Position;
        var builder = new StringBuilder();
        IBisLexer<TPreProcTypes>.TokenMatch token;
        while (!until(token = NextToken(lexer)))
        {
            builder.Append(token.TokenText);
        }

        return CreateTokenMatch(start..lexer.Position, builder.ToString(), asToken);
    }

    protected virtual void OnTokenMatchedHandler(IBisLexer<TPreProcTypes>.TokenMatch match, IBisLexer<TPreProcTypes> lexer) => OnTokenMatched?.Invoke(match, lexer);

    protected void AddPreviousMatch(IBisLexer<TPreProcTypes>.TokenMatch match) => previousMatches.Add(match);


    protected static IBisLexer<TPreProcTypes>.TokenDefinition CreateTokenDefinition(string debugName, TPreProcTypes tokenType, short tokenWeight) =>
        new() { DebugName = debugName, TokenId = tokenType, TokenWeight = tokenWeight };


    protected static IBisLexer<TPreProcTypes>.TokenMatch CreateTokenMatch(Range tokenRange, string str, IBisLexer<TPreProcTypes>.TokenDefinition tokenDef) =>
        new() {
            Success = true,
            TokenLength = tokenRange.End.Value - tokenRange.Start.Value + 1,
            TokenPosition = tokenRange.Start.Value,
            TokenText = str,
            TokenType = tokenDef
        };

}
