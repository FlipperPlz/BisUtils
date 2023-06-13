using BisUtils.Bank.Model.Stubs;
using BisUtils.Core.Binarize.Utils;
using BisUtils.Core.IO;

namespace BisUtils.Bank.Model.Entry;

public class PboVersionEntry : PboEntry
{
    public new string FileName { get; } = "$PROPERTIES$";

    public PboVersionEntry(
        string fileName = "",
        PboEntryMime mime = PboEntryMime.Version, 
        long originalSize = 0,
        long offset = 0,
        long timeStamp = 0, 
        long dataSize = 0
    ) : base(fileName, mime, originalSize, offset, timeStamp, dataSize)
    {
    }

    public PboVersionEntry(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
    }

    public override BinarizationResult Binarize(BisBinaryWriter writer, PboOptions options)
    {
        var result = base.Binarize(writer, options);
        if (result.IsNotValid) return result;
        //TODO: Write version properties
        return result;
    }

    public override BinarizationResult Debinarize(BisBinaryReader reader, PboOptions options)
    {
        var result = base.Debinarize(reader, options);
        if (result.IsNotValid) return result;
        //TODO: Read version properties


        return result;
    }

    public override bool Validate(PboOptions options)
    {
        //TODO: Validate using options
        throw new NotImplementedException();
    }
}