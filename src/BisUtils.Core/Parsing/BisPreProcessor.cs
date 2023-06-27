namespace BisUtils.Core.Parsing;

using System.Text;
using FResults;
using Lexer;

public interface IBisPreProcessor<TPreProcTypes> : IBisLexer<TPreProcTypes> where TPreProcTypes : Enum
{
    public Result EvaluateLexer<T>(BisLexer<T> lexer, StringBuilder? builder) where T : Enum;
}

public abstract class BisPreProcessor<TPreProcTypes> : IBisPreProcessor<TPreProcTypes> where TPreProcTypes : Enum
{
    public event IBisLexer<TPreProcTypes>.TokenMatched? OnTokenMatched;
    public abstract IEnumerable<IBisLexer<TPreProcTypes>.TokenDefinition> TokenTypes { get; }
    public abstract IBisLexer<TPreProcTypes>.TokenDefinition EOFToken { get; }
    public IBisLexer<TPreProcTypes>.TokenDefinition? ErrorToken => null;
    public IEnumerable<IBisLexer<TPreProcTypes>.TokenMatch> PreviousMatches => previousMatches;
    private readonly List<IBisLexer<TPreProcTypes>.TokenMatch> previousMatches = new();


    public IBisLexer<TPreProcTypes>.TokenMatch? PreviousMatch() => previousMatches.LastOrDefault();
    public IBisLexer<TPreProcTypes>.TokenMatch NextToken<T>(BisLexer<T> lexer) where T : Enum
    {
        var value = GetNextToken(lexer);
        OnTokenMatchedHandler(value, this);
        return value;
    }

    public abstract Result EvaluateLexer<T>(BisLexer<T> lexer, StringBuilder? builder) where T : Enum;
    protected abstract IBisLexer<TPreProcTypes>.TokenMatch GetNextToken<T>(BisLexer<T> lexer) where T : Enum;

    public void TokenizeUntilEnd<T>(BisLexer<T> lexer) where T : Enum
    {
        while (NextToken(lexer) != EOFToken.TokenId)
        {
        }
    }

    protected virtual void OnTokenMatchedHandler(IBisLexer<TPreProcTypes>.TokenMatch match, IBisLexer<TPreProcTypes> lexer)
    {
        previousMatches.Add(match);
        OnTokenMatched?.Invoke(match, lexer);
    }

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
