namespace BisUtils.Core.Cloning;

public interface IBisCloneable<out T> : ICloneable
{
    T BisClone();
    object ICloneable.Clone() => BisClone;
}
