namespace BisUtils.EnConfig.Tokens;

using Core.Singleton;
using EnLex;
using EnLex.Tokens;

public sealed class EnConfigTokens : EnfusionTokenSet<EnConfigTokens>
{
    public static EnConfigTokens Instance => BisSingletonProvider.LocateInstance<EnConfigTokens>();

}
