using BisUtils.Core.Options;

namespace BisUtils.Core.Binarize.Validatable;

public interface IValidatable<in T> where T : IBisOptions
{
    public bool Validate(T options);
}