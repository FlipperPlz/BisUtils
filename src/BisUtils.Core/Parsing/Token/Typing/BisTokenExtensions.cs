namespace BisUtils.Core.Parsing.Token.Typing;

using System.Reflection;

public static class BisTokenExtensions
{
    private static readonly Dictionary<Type, IBisTokenTypeSet> TokenSets = new();
    private static readonly Dictionary<IBisTokenTypeSet, IEnumerable<IBisTokenType>> TokenTypes = new();

    public static IEnumerable<IBisTokenType> GetTokens<T>(this IBisTokenTypeSet tokenSet) where T : IBisTokenTypeSet<T>
    {
        var tokenSetType = typeof(T);
        TokenSets.TryAdd(tokenSetType, tokenSet);
        return LocateTokens<T>(tokenSet, tokenSetType);
    }

    public static T FindTokenSet<T>() where T : BisTokenTypeSet<T>, new()
    {
        var tokenSetType = typeof(T);

        if (TokenSets.TryGetValue(tokenSetType, out var tokenSet))
        {
            return (T) tokenSet;
        }

        var set = new T();
        TokenSets.TryAdd(tokenSetType, set);
        return set;
    }

    private static IEnumerable<IBisTokenType> LocateTokens<T>(IBisTokenTypeSet tokenSet, IReflect setType) where T : IBisTokenTypeSet<T>
    {
        if (TokenTypes.TryGetValue(tokenSet, out var value))
        {
            return value;
        }

        return TokenTypes[tokenSet] = setType.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(it => it.IsInitOnly && it.FieldType == typeof(BisTokenType)).Select(it => it.GetValue(null))
            .Cast<IBisTokenType>();
    }

}
