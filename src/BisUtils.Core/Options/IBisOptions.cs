namespace BisUtils.Core.Options;

/// <summary>
/// Base interface for defining an option set to be used by the framework.
/// There should be no complex types defined as they cannot be serialized
/// ([string, int, float, object-array] only)
/// (todo: maybe later add a param serializable attribute with a delegate for reading and writing)
/// </summary>
public interface IBisOptions
{
}
