namespace BisUtils.Core.Parsing;

using System.Text;
using FResults;
using Lexer;

#pragma warning disable CA1000

/// <summary>
/// The `IBisParser` interface defines a simple generic parser for bi-directional information systems.
/// </summary>
/// <typeparam name="TAstNode">The type of the abstract syntax tree node to be output by the parser.</typeparam>
/// <typeparam name="TLexer">The type of the lexer used by the parser. Must inherit from `BisLexer`.</typeparam>
/// <typeparam name="TTypes">The type of the token types used by the parser.</typeparam>
public interface IBisParser<TAstNode, in TLexer, TTypes> where TLexer : BisLexer<TTypes> where TTypes : Enum
{
    /// <summary>
    /// Parses a lexer and outputs an abstract syntax tree node.
    /// </summary>
    /// <param name="node">The output abstract syntax tree node.</param>
    /// <param name="lexer">The lexer to parse.</param>
    /// <returns>A result of the parsing operation.</returns>
    public Result Parse(out TAstNode? node, TLexer lexer);
}

/// <summary>
/// The `IBisParser` interface extends `IBisParser` with support for preprocessing. It defines a generic parser for bi-directional information systems.
/// </summary>
/// <typeparam name="TAstNode">The type of the abstract syntax tree node to be output by the parser.</typeparam>
/// <typeparam name="TLexer">The type of the lexer used by the parser. Must inherit from `BisLexer`.</typeparam>
/// <typeparam name="TTypes">The type of the token types used by the parser.</typeparam>
/// <typeparam name="TPreprocessor">The type of the preprocessor used by the parser. Must inherit from `BisPreProcessorBase` and have a parameterless constructor.</typeparam>
public interface IBisParser<TAstNode, in TLexer, TTypes, in TPreprocessor> : IBisParser<TAstNode, TLexer, TTypes> where TLexer : BisLexer<TTypes> where TPreprocessor : BisPreProcessorBase, new() where TTypes : Enum
{
    /// <summary>
    /// Processes a lexer using a preprocessor and outputs an abstract syntax tree node.
    /// </summary>
    /// <param name="node">The output abstract syntax tree node.</param>
    /// <param name="lexer">The lexer to parse.</param>
    /// <param name="preprocessor">The preprocessor to use. If not specified, a new instance of `TPreprocessor` is created.</param>
    /// <returns>A result of the parsing and preprocessing operation.</returns>
    public Result ProcessAndParse
    (
        out TAstNode? node,
        TLexer lexer,
        TPreprocessor? preprocessor = null
    )
    {
        var builder = new StringBuilder();
        preprocessor ??= new TPreprocessor();
        preprocessor.EvaluateLexer(lexer, builder);
        lexer.ResetLexer(builder.ToString());
        return Parse(out node, lexer);
    }
}

