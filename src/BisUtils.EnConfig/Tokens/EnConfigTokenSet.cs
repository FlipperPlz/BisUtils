namespace BisUtils.EnConfig.Tokens;

using Core.Singleton;
using EnLex;
using EnLex.Tokens;

public sealed class EnConfigTokenSet : EnfusionTokenSet
{
    public static EnConfigTokenSet Instance => BisSingletonProvider.LocateInstance<EnConfigTokenSet>();

}
