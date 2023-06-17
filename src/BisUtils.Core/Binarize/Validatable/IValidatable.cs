namespace BisUtils.Core.Binarize.Validatable;

using Core.Options;
using FResults;

public interface IValidatable<in T> where T : IBisOptions
{
    public Result Validate(T options);
}
