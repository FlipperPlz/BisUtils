namespace BisUtils.Core.Binarize;

using Options;

/// <summary>
/// Defines an object that supports both binarization and debinarization using the same options.
/// This interface is suitable for types needing to provide custom logic for both binary serialization and deserialization.
/// </summary>
/// <typeparam name="TOptions">Specifies the type of the binarization/debinarization options to be used by the Binarize and Debinarize methods.</typeparam>
public interface IBinaryObject<in TOptions> : IBinarizable<TOptions>, IDebinarizable<TOptions> where TOptions : IBinarizationOptions
{
}
