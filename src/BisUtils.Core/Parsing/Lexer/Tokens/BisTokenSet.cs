namespace BisUtils.Core.Parsing.Lexer.Tokens;

using System.Collections;
using System.Reflection;
using Misc;


#pragma warning disable CA1000
//Fuck your conventions, I do what I want
public interface IBisTokenSet<TSelf> : IEnumerable<IBisToken>, IBisSelfHolder<TSelf> where TSelf : IBisTokenSet<TSelf>
{
    public static abstract IEnumerable<IBisToken> Types { get; }
}

public abstract class BisTokenSet<TSelf> : IBisTokenSet<TSelf> where TSelf : BisTokenSet<TSelf>
{

    public static IEnumerable<IBisToken> Types => typeof(TSelf).GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(it => it.IsInitOnly && it.FieldType == typeof(BisToken)).Select(it => it.GetValue(null))
        .Cast<IBisToken>();

    public IEnumerator<IBisToken> GetEnumerator() => Types.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
#pragma warning restore CA1000
