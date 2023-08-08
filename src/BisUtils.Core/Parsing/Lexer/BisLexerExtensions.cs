namespace BisUtils.Core.Parsing.Lexer;

public static class BisLexerExtensions
{
    public static int GetFarthestIndexed(this IBisLexer lexer)
    {
        if (!lexer.PreviousMatches.Any())
        {
            return -1;
        }

        var last = lexer.PreviousMatches.LastOrDefault();

        return last.TokenPosition + last.TokenLength;
    }

}
