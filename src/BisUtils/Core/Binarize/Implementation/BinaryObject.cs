namespace BisUtils.Core.Binarize.Implementation;

using BisUtils.Core.Binarize.Options;
using BisUtils.Core.IO;
using FResults;

public abstract class BinaryObject<T> : IBinaryObject<T> where T : IBinarizationOptions
{
    public Result? LastResult { get; protected set; }

    protected BinaryObject(BisBinaryReader reader, T options)
    {
    }

    protected BinaryObject()
    {

    }

    public abstract Result Binarize(BisBinaryWriter writer, T options);
    public abstract Result Debinarize(BisBinaryReader reader, T options);
}
