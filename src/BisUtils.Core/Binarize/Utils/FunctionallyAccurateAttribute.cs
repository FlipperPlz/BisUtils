namespace BisUtils.Core.Binarize.Utils;

using Core.Options;

/// <summary>
/// Marks the parameter on which it is applied as having a default value in implementations of <see cref="IBisOptions"/>>.
/// This attribute can only be applied to parameters.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class FunctionallyAccurateAttribute : Attribute
{
}
