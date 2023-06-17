namespace BisUtils.Core.Binarize.Implementation;

using FResults;
using IO;
using Options;

public abstract class BinaryObject<T> : IBinaryObject<T> where T : IBinarizationOptions
{
    protected BinaryObject(BisBinaryReader reader, T options)
    {
    }

    protected BinaryObject()
    {
    }

    public Result? LastResult { get; protected set; }

    public abstract Result Binarize(BisBinaryWriter writer, T options);
    public abstract Result Debinarize(BisBinaryReader reader, T options);
}
