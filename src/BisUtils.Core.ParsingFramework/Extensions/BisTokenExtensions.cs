namespace BisUtils.Core.ParsingFramework.Extensions;

using System.Reflection;
using Singleton;
using Tokens.Match;
using Tokens.Type;
using Tokens.TypeSet;

public static class BisTokenExtensions
{
    private static readonly Dictionary<IBisTokenTypeSet, IEnumerable<IBisTokenType>> TokenTypes = new();

    public static IEnumerable<IBisTokenType> GetTokens<T>(this BisTokenTypeSet<T> tokenSet) where T : IBisTokenTypeSet<T>, new()
    {
        var tokenSetType = typeof(T);
        BisSingletonProvider.AddInstance(tokenSet);
        return LocateTokens<T>(tokenSet, tokenSetType);
    }

    public static T FindTokenSet<T>() where T : IBisTokenTypeSet<T>, new() =>
        BisSingletonProvider.LocateInstance<T>();


    private static IEnumerable<IBisTokenType> LocateTokens<T>(this IBisTokenTypeSet tokenSet, IReflect setType) where T : IBisTokenTypeSet<T>, new()
    {
        if (TokenTypes.TryGetValue(tokenSet, out var value))
        {
            return value;
        }

        return TokenTypes[tokenSet] = setType.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(it => it.IsInitOnly && it.FieldType == typeof(BisTokenType)).Select(it => it.GetValue(null))
            .Cast<IBisTokenType>();
    }


    public static Range GetTokenLocation(this IBisTokenMatch match) =>
        match.TokenPosition..(match.TokenPosition + match.TokenLength);

}
