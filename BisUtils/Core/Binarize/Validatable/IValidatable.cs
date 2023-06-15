using BisUtils.Core.Options;

namespace BisUtils.Core.Binarize.Validatable;

using FResults;

public interface IValidatable<in T> where T : IBisOptions
{
    public Result Validate(T options);
}
