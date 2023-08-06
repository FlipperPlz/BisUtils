namespace BisUtils.Core.Parsing.Lexer.Tokens;

using System.Reflection;

public static class BisTokenExtensions
{
    private static readonly Dictionary<Type, IEnumerable<IBisToken>> TokenSets = new();

    public static IEnumerable<IBisToken> GetTokens<T>(this T tokenSet) where T : IBisTokenSet
    {

        var type = typeof(T);
        if (TokenSets.TryGetValue(type, out var value))
        {
            return value;
        }

        return TokenSets[type] = type.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(it => it.IsInitOnly && it.FieldType == typeof(BisToken)).Select(it => it.GetValue(null))
            .Cast<IBisToken>();
    }
}
