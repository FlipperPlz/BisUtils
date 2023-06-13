using BisUtils.Core.Binarize;
using BisUtils.Core.Binarize.Utils;
using BisUtils.Core.Binarize.Validatable;
using BisUtils.Core.Family;
using BisUtils.Core.IO;

namespace BisUtils.Bank.Model;

public interface IPboVFSEntry : IStrictBinaryObject<PboOptions>, IFamilyChild, ICloneable
{

    bool IValidatable<PboOptions>.Validate(PboOptions options)
    {
        throw new NotImplementedException();
    }

    BinarizationResult IBinarizable<PboOptions>.Binarize(BisBinaryWriter writer, PboOptions options)
    {
        throw new NotImplementedException();
    }

    BinarizationResult IDebinarizable<PboOptions>.Debinarize(BisBinaryReader reader, PboOptions options)
    {
        throw new NotImplementedException();
    }
}