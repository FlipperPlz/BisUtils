namespace BisUtils.Core.Binarize.Implementation;

using FResults;
using IO;
using Options;

public abstract class StrictBinaryObject<T> : BinaryObject<T>, IStrictBinaryObject<T> where T : IBinarizationOptions
{
    protected StrictBinaryObject(BisBinaryReader reader, T options) : base(reader, options)
    {
    }

    protected StrictBinaryObject()
    {
    }

    public abstract Result Validate(T options);
}
