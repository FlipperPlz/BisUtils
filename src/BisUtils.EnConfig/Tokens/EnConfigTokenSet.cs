namespace BisUtils.EnConfig.Tokens;

using Core.Singleton;
using EnLex;
using EnLex.Tokens;

public sealed class EnConfigTokenSet : EnfusionTokenSet<EnConfigTokenSet>
{
    public static EnConfigTokenSet Instance => BisSingletonProvider.LocateInstance<EnConfigTokenSet>();

}
