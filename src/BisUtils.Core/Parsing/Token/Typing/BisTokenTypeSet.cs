namespace BisUtils.Core.Parsing.Token.Typing;

using System.Collections;using Singleton;
using Tokens;

public interface IBisTokenTypeSet : IEnumerable<IBisTokenType>
{
}

public interface IBisTokenTypeSet<out TSelf> : IBisTokenTypeSet where TSelf : IBisTokenTypeSet<TSelf>
{

}


public class BisTokenTypeSet<TSelf> : BisSingleton, IBisTokenTypeSet<TSelf> where TSelf : IBisTokenTypeSet<TSelf>
{
    protected const string ToComplex = "to complex";


    public static readonly IBisTokenType BisEOF =
        new BisCustomEOLTokenType("__bisnull__", "null");

    public IEnumerator<IBisTokenType> GetEnumerator() => this.GetTokens<TSelf>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
