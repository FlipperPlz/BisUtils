using BisUtils.Core.Binarize.Utils;
using BisUtils.Core.IO;

namespace BisUtils.Bank.Model.Stubs;

using FluentResults;

public abstract class PboEntry : PboVFSEntry
{
    public new string EntryName { get; set; } = "New File";
    public PboEntryMime EntryMime { get; set; } = PboEntryMime.Decompressed;
    public long OriginalSize { get; set; }
    public long Offset { get; set; }
    public long TimeStamp { get; set; }
    public long DataSize { get; set; }

    protected PboEntry(
        string fileName,
        PboEntryMime mime,
        long originalSize,
        long offset,
        long timeStamp,
        long dataSize
    ) : base(fileName)
    {
        EntryMime = mime;
        OriginalSize = originalSize;
        Offset = offset;
        TimeStamp = timeStamp;
        DataSize = dataSize;
    }

    protected PboEntry(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
    }

    public bool IsEmptyMeta() =>
        OriginalSize == 0 &&
        Offset == 0 &&
        TimeStamp == 0 &&
        DataSize == 0;

    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        //TODO: Binarize with options

        throw new NotImplementedException();
    }

    public override Result Debinarize(BisBinaryReader reader, PboOptions options)
    {

        var result = base.Debinarize(reader, options);
        if (result.IsFailed)
        {
            return result;
        }
        //write mime
        //write originalSize
        //write offset
        //write timestamp
        //write size

        return result;
    }
}
