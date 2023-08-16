namespace BisUtils.Core.ParsingFramework.Lexer;

using Tokens.TypeSet;

public interface IBisLexerAbs<out TTokens> : IBisLexer<TTokens> where TTokens : BisTokenTypeSet<TTokens>, new()
{
}

