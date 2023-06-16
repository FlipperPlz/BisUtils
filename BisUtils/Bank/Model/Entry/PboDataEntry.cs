namespace BisUtils.Bank.Model.Entry;

using Alerts.Warnings;
using Core.Family;
using Core.IO;
using FResults;
using Stubs;

public interface IPboDataEntry : IPboEntry
{
    Stream EntryData { get; }

    new IFamilyNode? Node => PboFile;

    IFamilyParent? IFamilyChild.Parent => ParentDirectory;
}

public class PboDataEntry : IPboDataEntry
{
    public IPboFile? PboFile { get; set; }

    public IPboDirectory? ParentDirectory { get; set; }

    public string EntryName { get; set; } = "";

    public PboEntryMime EntryMime { get; set; } = PboEntryMime.Decompressed;

    public long OriginalSize { get; set; }

    public long Offset { get; set; }

    public long TimeStamp { get; set; }

    public long DataSize { get; set; }

    public Stream EntryData { get; set; } = Stream.Null;

    public Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        writer.WriteAsciiZ(EntryName, options.Charset);
        writer.Write((long) EntryMime);
        writer.Write(OriginalSize);
        writer.Write(Offset);
        writer.Write(TimeStamp);
        writer.Write(DataSize);
        return Result.ImmutableOk();
    }

    public Result Validate(PboOptions options)
    {
        var results = new List<Result>();

        switch (EntryMime)
        {
            case PboEntryMime.Encrypted:
                results.Add(new Result().WithWarning(new PboEncryptedEntryWarning(!options.AllowEncrypted)));
                break;
            case PboEntryMime.Version:
                results.Add(new Result().WithWarning(new PboImproperMimeWarning(!options.AllowVersionMimeOnData, typeof(IPboDataEntry))));
                break;
            case PboEntryMime.Decompressed:
            case PboEntryMime.Compressed:
            default:
                break;
        }

        if (EntryName.Length == 0)
        {
            results.Add(new Result().WithWarning(new PboUnnamedEntryWarning(!options.AllowUnnamedDataEntries, typeof(IPboDataEntry))));
        }

        //TODO: Check Lengths/Sizes


        return Result.Merge(results);

    }

    public Result Debinarize(BisBinaryReader reader, PboOptions options)
    {
        throw new NotImplementedException();
    }
}
