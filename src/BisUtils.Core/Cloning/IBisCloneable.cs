namespace BisUtils.Core.Cloning;

public interface IBisCloneable<out T> : ICloneable
{
    object ICloneable.Clone() => BisClone;
    T BisClone();
}
