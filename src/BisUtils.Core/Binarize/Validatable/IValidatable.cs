namespace BisUtils.Core.Binarize.Validatable;

using BisUtils.Core.Options;
using FResults;

public interface IValidatable<in T> where T : IBisOptions
{
    public Result Validate(T options);
}
