namespace BisUtils.Core.Parsing.Lexer.Tokens;

using System.Collections;

public interface IBisTokenSet : IEnumerable<IBisToken>
{
}

public abstract class BisTokenSet : IBisTokenSet
{
    public IEnumerator<IBisToken> GetEnumerator() => this.GetTokens().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
