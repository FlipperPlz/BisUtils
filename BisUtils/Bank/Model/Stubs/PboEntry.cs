using BisUtils.Core.Binarize.Utils;
using BisUtils.Core.IO;

namespace BisUtils.Bank.Model.Stubs;

public abstract class PboEntry : PboVFSEntry
{
    public string FileName { get; set; } = "New File";
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
    ) : base()
    {
        FileName = fileName;
        EntryMime = mime;
        OriginalSize = originalSize;
        Offset = offset;
        TimeStamp = timeStamp;
        DataSize = dataSize;
    }
    
    protected PboEntry(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
    }

    public override BinarizationResult Binarize(BisBinaryWriter writer, PboOptions options)
    {
        //TODO: Binarize with options
        throw new NotImplementedException();
    }

    public override BinarizationResult Debinarize(BisBinaryReader reader, PboOptions options)
    {
        //TODO: Deinarize with options
        throw new NotImplementedException();
    }
}