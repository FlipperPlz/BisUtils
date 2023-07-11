namespace BisUtils.Bank.Model.Entry;

using Alerts.Warnings;
using Core.Binarize.Exceptions;
using Core.IO;
using Enumerations;
using FResults;
using FResults.Extensions;
using FResults.Reasoning;
using Options;
using Stubs;
using Utils;

public interface IPboDataEntry : IPboEntry
{
    Stream EntryData { get; }

    void ExpandDirectoryStructure();
}

public class PboDataEntry : PboEntry, IPboDataEntry
{
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

    public Stream EntryData { get; set; } = Stream.Null;

    public void ExpandDirectoryStructure()
    {
        ArgumentNullException.ThrowIfNull(PboFile, "When expanding a Pbo Entry, The node must be established");

        var normalizePath = EntryName = PboPathUtilities.NormalizePboPath(EntryName);

        if (!EntryName.Contains('\\'))
        {
            return;
        }

        EntryName = PboPathUtilities.GetFilename(EntryName);

        ParentDirectory = PboFile.CreateDirectory(PboPathUtilities.GetParent(normalizePath), PboFile);
        ParentDirectory.PboEntries.Add(this);
    }

    public sealed override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        LastResult = base.Binarize(writer, options);
        writer.Write((long)EntryMime);
        writer.Write(OriginalSize);
        writer.Write(Offset);
        writer.Write(TimeStamp);
        writer.Write(DataSize);

        return LastResult;
    }

    public sealed override Result Validate(PboOptions options)
    {
        LastResult = Result.Ok();

        switch (EntryMime)
        {
            case PboEntryMime.Encrypted:
                LastResult.WithWarning(new PboEncryptedEntryWarning(!options.AllowEncrypted));
                break;
            case PboEntryMime.Version:
                LastResult.WithWarning(new PboImproperMimeWarning(!options.AllowVersionMimeOnData,
                    typeof(IPboDataEntry)));
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

        return LastResult;
    }

    public sealed override Result Debinarize(BisBinaryReader reader, PboOptions options)
    {
        LastResult = base.Debinarize(reader, options);
        EntryMime = (PboEntryMime)reader.ReadInt32(); // TODO WARN/ERROR then recover
        OriginalSize = reader.ReadInt32();
        TimeStamp = reader.ReadInt32();
        Offset = reader.ReadInt32();
        DataSize = reader.ReadInt32();

        if (!options.IgnoreValidation)
        {
            LastResult = Result.Merge(LastResult, Validate(options));
        }

        return LastResult;
    }


    public void SynchronizeMetaWithStream() => OriginalSize = (int)EntryData.Length;
}
