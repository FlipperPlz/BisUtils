namespace BisUtils.Core.Parsing;

using System.Text;
using FResults;
using Lexer;

/// <summary>
/// Interface for the BIS PreProcessor objects.
/// </summary>
public interface IBisPreProcessorBase
{
    /// <summary>
    /// Evaluate the lexer and if present, append outcome to the StringBuilder instance.
    /// </summary>
    /// <param name="lexer">IBisMutableStringStepper instance.</param>
    /// <param name="builder">StringBuilder instance.</param>
    /// <returns>Returns Result instance. </returns>
    public Result EvaluateLexer(IBisMutableStringStepper lexer, StringBuilder? builder);

}

/// <summary>
/// Abstract base class for the BIS PreProcessor objects. Implements IBisPreProcessorBase.
/// </summary>
public abstract class BisPreProcessorBase : IBisPreProcessorBase
{
    public abstract Result EvaluateLexer(IBisMutableStringStepper lexer, StringBuilder? builder);
}


/// <summary>
/// Abstract base class for the generic BIS PreProcessor.
/// <para>
/// Provides the base functionality to be extended by specific preprocessors in the working context.
/// </para>
/// </summary>
/// <typeparam name="TPreProcTypes">A set of types which the PreProcessor will work with, represented as an enumeration.</typeparam>
public abstract class BisPreProcessor<TPreProcTypes> : BisPreProcessorBase, IBisLexerOld<TPreProcTypes> where TPreProcTypes : Enum
{

    /// <summary>
    /// Event that fires when a token match occurs.
    /// </summary>
    public event IBisLexerOld<TPreProcTypes>.TokenMatched? OnTokenMatched;

    /// <summary>
    /// Provides definitions for token types.
    /// </summary>
    public abstract IEnumerable<IBisLexerOld<TPreProcTypes>.TokenDefinition> TokenTypes { get; }

    public abstract IBisLexerOld<TPreProcTypes>.TokenDefinition EOFToken { get; }

    public IBisLexerOld<TPreProcTypes>.TokenDefinition? ErrorToken => null;

    public IEnumerable<IBisLexerOld<TPreProcTypes>.TokenMatch> PreviousMatches => previousMatches;

    private readonly List<IBisLexerOld<TPreProcTypes>.TokenMatch> previousMatches = new();

    public IBisLexerOld<TPreProcTypes>.TokenMatch? PreviousMatch() => previousMatches.LastOrDefault();

    /// <summary>
    /// Produces the next available token from the provided lexer object. The method internally
    /// calls the abstract GetNextToken() and then raises an OnTokenMatched event with the
    /// retrieved token as an argument.
    /// </summary>
    /// <param name="lexer">The lexer (an instance of IBisMutableStringStepper)</param>
    /// <returns>The next available token</returns>
    public IBisLexerOld<TPreProcTypes>.TokenMatch NextToken(IBisMutableStringStepper lexer)
    {
        var value = GetNextToken(lexer);
        OnTokenMatchedHandler(value, this);
        return value;
    }

    /// <summary>
    /// Gets the next available token given a mutable string stepper. This function should
    /// only ever be accessed my NextToken to prevent token-swallowing.
    /// DO NOT CALL THIS METHOD! HANDS OFF!!!
    /// </summary>
    /// <param name="lexer">Contextual mutable string stepper instance.</param>
    /// <returns>The next available token.</returns>
    protected abstract IBisLexerOld<TPreProcTypes>.TokenMatch GetNextToken(IBisMutableStringStepper lexer);

    public void TokenizeUntilEnd<T>(BisLexerOld<T> lexerOld) where T : Enum
    {
        while (NextToken(lexerOld) != EOFToken.TokenId)
        {
        }
    }

    /// <summary>
    /// Tokenizes lexer input until a certain condition (expressed as a Func delegate) is met.
    /// Populates a string builder with all tokens encountered until the condition becomes true.
    /// The final string representation of accumulated tokens is used to create a new token match
    /// which is returned.
    /// </summary>
    /// <typeparam name="TTypes">The enumeration that represents the set of types of tokens the lexer will work with.</typeparam>
    /// <param name="lexerOld">The lexer object (an instance of BisLexer<![CDATA[<]]>TTypes<![CDATA[>]]>)</param>
    /// <param name="until">A function that determines when the tokenization process should stop.</param>
    /// <param name="asToken">The definition of the token to be generated from the accumulated text.</param>
    /// <returns>A new token match that encapsulates the accumulated text.</returns>
    public IBisLexerOld<TPreProcTypes>.TokenMatch TokenizeUntil<TTypes>(
        BisLexerOld<TTypes> lexerOld,
        Func<IBisLexerOld<TPreProcTypes>.TokenMatch, bool> until, IBisLexerOld<TPreProcTypes>.TokenDefinition asToken) where TTypes : Enum
    {
        var start = lexerOld.Position;
        var builder = new StringBuilder();
        IBisLexerOld<TPreProcTypes>.TokenMatch token;
        while (!until(token = NextToken(lexerOld)))
        {
            builder.Append(token.TokenText);
        }

        return CreateTokenMatch(start..lexerOld.Position, builder.ToString(), asToken);
    }

    protected virtual void OnTokenMatchedHandler(IBisLexerOld<TPreProcTypes>.TokenMatch match, IBisLexerOld<TPreProcTypes> lexerOld) => OnTokenMatched?.Invoke(match, lexerOld);

    /// <summary>
    /// Adds a new match to the list of previous matches.
    /// </summary>
    /// <param name="match">Match instance to be added.</param>
    protected void AddPreviousMatch(IBisLexerOld<TPreProcTypes>.TokenMatch match) => previousMatches.Add(match);


    protected static IBisLexerOld<TPreProcTypes>.TokenDefinition CreateTokenDefinition(string debugName, TPreProcTypes tokenType, short tokenWeight) =>
        new() { DebugName = debugName, TokenId = tokenType, TokenWeight = tokenWeight };


    protected static IBisLexerOld<TPreProcTypes>.TokenMatch CreateTokenMatch(Range tokenRange, string str, IBisLexerOld<TPreProcTypes>.TokenDefinition tokenDef) =>
        new() {
            Success = true,
            TokenLength = tokenRange.End.Value - tokenRange.Start.Value + 1,
            TokenPosition = tokenRange.Start.Value,
            TokenText = str,
            TokenType = tokenDef
        };

}
