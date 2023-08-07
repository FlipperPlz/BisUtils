namespace BisUtils.Core.Parsing.Token;

using System.Collections;

public interface IBisTokenSet : IEnumerable<IBisToken>
{
}

public abstract class BisTokenSet<TSelf> : IBisTokenSet
{
    public IEnumerator<IBisToken> GetEnumerator() => this.GetTokens().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
