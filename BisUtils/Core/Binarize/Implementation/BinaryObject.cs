using BisUtils.Core.Binarize.Exceptions;
using BisUtils.Core.Binarize.Options;
using BisUtils.Core.Binarize.Utils;
using BisUtils.Core.IO;

namespace BisUtils.Core.Binarize.Implementation;

public abstract class BinaryObject<T> : IBinaryObject<T> where T : IBinarizationOptions
{
    protected BinaryObject(BisBinaryReader reader, T options)
    {
        var result = Debinarize(reader, options);
        if (!result.IsValid) throw new DebinarizeFailedException(result.Errors);
    }

    protected BinaryObject()
    {
        
    }

    public abstract BinarizationResult Binarize(BisBinaryWriter writer, T options);
    public abstract BinarizationResult Debinarize(BisBinaryReader reader, T options);
}