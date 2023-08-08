namespace BisUtils.Core.Parsing.Lexer;

public static class BisLexerExtensions
{
    public static int GetFarthestIndexed(this IBisLexer lexer)
    {
        var last = lexer.PreviousMatches.LastOrDefault();
        if(last is null)
        {
            return -1;
        }

        return last.TokenPosition + last.TokenLength;
    }

}
