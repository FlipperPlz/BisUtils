namespace BisUtils.Core.Parsing.Token;

using System.Reflection;

public static class BisTokenExtensions
{
    private static readonly Dictionary<Type, IEnumerable<IBisToken>> TokenSets = new();

    public static IEnumerable<IBisToken> GetTokens<T>(this T _) where T : IBisTokenSet => LocateTokens(typeof(T));

    public static IEnumerable<IBisToken> LocateTokens(Type set)
    {
        if (TokenSets.TryGetValue(set, out var value))
        {
            return value;
        }

        return TokenSets[set] = set.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(it => it.IsInitOnly && it.FieldType == typeof(BisToken)).Select(it => it.GetValue(null))
            .Cast<IBisToken>();
    }
}
