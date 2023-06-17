namespace BisUtils.Bank.Model.Entry;

using System.Diagnostics;
using BisUtils.Bank.Alerts.Warnings;
using BisUtils.Core.Binarize.Exceptions;
using BisUtils.Core.IO;
using BisUtils.Bank.Model.Stubs;

using FResults;
using FResults.Extensions;
using FResults.Reasoning;
using Options;
using Utils;

public interface IPboDataEntry : IPboEntry
{
    Stream EntryData { get; }

    void ExpandDirectoryStructure();
}

public class PboDataEntry : PboEntry, IPboDataEntry
{
    public Stream EntryData { get; set; } = Stream.Null;

    public void ExpandDirectoryStructure()
    {
#if DEBUG
        var watch = Stopwatch.StartNew();
#endif
        ArgumentNullException.ThrowIfNull(PboFile, "When expanding a Pbo Entry, The node must be established");

        var normalizePath = EntryName = PboPathUtilities.NormalizePboPath(EntryName);

        if (!EntryName.Contains('\\'))
        {
            return;
        }

        EntryName = PboPathUtilities.GetFilename(EntryName);

        ParentDirectory = PboFile.CreateDirectory(PboPathUtilities.GetParent(normalizePath), PboFile);
        ParentDirectory.PboEntries.Add(this);

#if DEBUG
        watch.Stop();
        Console.WriteLine($"(ExpandDirectoryStructure) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif
    }

    public PboDataEntry
    (
        IPboFile? file,
        IPboDirectory? parent,
        string fileName,
        PboEntryMime mime,
        int originalSize,
        int offset,
        int timeStamp,
        int dataSize
    ) : base(file, parent, fileName, mime, originalSize, offset, timeStamp, dataSize)
    {
    }

    public PboDataEntry(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
        Debinarize(reader, options);
        if (LastResult!.IsFailed)
        {
            throw new DebinarizeFailedException(LastResult.ToString());
        }
    }

    public sealed override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {

#if DEBUG
        var watch = Stopwatch.StartNew();
#endif

        LastResult = base.Binarize(writer, options);
        writer.Write((long) EntryMime);
        writer.Write(OriginalSize);
        writer.Write(Offset);
        writer.Write(TimeStamp);
        writer.Write(DataSize);

#if DEBUG
        watch.Stop();
        Console.WriteLine($"(PboDataEntry::Binarize) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif
        return LastResult;
    }

    public sealed override Result Validate(PboOptions options)
    {
#if DEBUG
        var watch = Stopwatch.StartNew();
#endif
        LastResult = Result.Ok();

        switch (EntryMime)
        {
            case PboEntryMime.Encrypted:
                LastResult.WithWarning(new PboEncryptedEntryWarning(!options.AllowEncrypted));
                break;
            case PboEntryMime.Version:
                LastResult.WithWarning(new PboImproperMimeWarning(!options.AllowVersionMimeOnData, typeof(IPboDataEntry)));
                break;
            case PboEntryMime.Decompressed:
            case PboEntryMime.Compressed:
            default:
                break;
        }

        if (EntryName.Length == 0)
        {
            LastResult.WithWarning(new PboUnnamedEntryWarning(!options.AllowUnnamedDataEntries, typeof(IPboDataEntry)));
        }

        if (EntryData.Length != DataSize && options.CurrentSection != PboSection.Header)
        {
            LastResult.WithWarning(new Warning
            {
                AlertScope = typeof(IPboDataEntry),
                AlertName = "EntryReadError",
                Message = "Incorrect Stream/DataSize Value.",
                IsError = !options.IgnoreInvalidStreamSize
            });
        }

        if (DataSize <= 0)
        {
            LastResult.WithWarning(new Warning
            {
                AlertScope = typeof(IPboDataEntry),
                AlertName = "EntryReadError",
                Message = "Incorrect DataSize Value.",
                IsError = !options.IgnoreInvalidStreamSize
            });
        }

        if (OriginalSize <= 0)
        {
            LastResult.WithWarning(new Warning
            {
                AlertScope = typeof(IPboDataEntry),
                AlertName = "EntryReadError",
                Message = "Incorrect OriginalSize Value.",
                IsError = !options.IgnoreInvalidStreamSize
            });
        }

#if DEBUG
        watch.Stop();
        Console.WriteLine($"(PboDataEntry::Validate) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif

        return LastResult;
    }

    public sealed override Result Debinarize(BisBinaryReader reader, PboOptions options)
    {
#if DEBUG
        var watch = Stopwatch.StartNew();
#endif


        LastResult = base.Debinarize(reader, options);
        EntryMime = (PboEntryMime) reader.ReadInt32();// TODO WARN/ERROR then recover
        OriginalSize = reader.ReadInt32();
        TimeStamp = reader.ReadInt32();
        Offset = reader.ReadInt32();
        DataSize = reader.ReadInt32();

        if (!options.IgnoreValidation)
        {
            LastResult = Result.Merge(LastResult, Validate(options));
        }


#if DEBUG
        watch.Stop();
        Console.WriteLine($"(PboDataEntry::Validate) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif

        return LastResult;
    }



    public void SynchronizeMetaWithStream() => OriginalSize = (int) EntryData.Length;
}
