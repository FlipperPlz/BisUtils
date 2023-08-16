namespace BisUtils.Core.ParsingFramework.Tokens.TypeSet;

using System.Collections;
using Extensions;
using Singleton;
using Type;
using Type.Types;

public class BisTokenTypeSet<TSelf> : BisSingleton, IBisTokenTypeSet<TSelf> where TSelf : IBisTokenTypeSet<TSelf>, new()
{
    protected const string ToComplex = "to complex";


    public static readonly IBisTokenType BisEOF =
        new BisCustomEOLTokenType("__bisnull__", "null");

    public BisTokenTypeSet()
    {

    }

    public IEnumerator<IBisTokenType> GetEnumerator() => this.GetTokens().GetEnumerator();



    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
