namespace BisUtils.Core.ParsingFramework.Tokens.TypeSet;

using Singleton;
using Type;

public interface IBisTokenTypeSet : IEnumerable<IBisTokenType>, IBisSingleton
{
}

public interface IBisTokenTypeSet<out TSelf> : IBisTokenTypeSet where TSelf : IBisTokenTypeSet<TSelf>
{

}
