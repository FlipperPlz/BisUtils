namespace BisUtils.Core.Cloning;

/// <summary>
/// Represents an interface for objects in the framework that can produce clones of themselves with
/// a specific type.
/// </summary>
/// <typeparam name="T">The type of the cloned object.</typeparam>
public interface IBisCloneable<out T> : ICloneable
{
    /// <summary>
    /// Creates a new object that is a clone of the current instance.
    /// This method is explicitly implemented to present ICloneable.Clone method to consumers.
    /// </summary>
    object ICloneable.Clone() => BisClone()!;

    /// <summary>
    /// Creates a new object that is a clone of the current instance with type T.
    /// </summary>
    /// <returns>A new object that is a clone of this instance with type T.</returns>
    T BisClone();
}
