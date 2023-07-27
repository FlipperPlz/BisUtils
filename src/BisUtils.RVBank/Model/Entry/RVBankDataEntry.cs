namespace BisUtils.RVBank.Model.Entry;

using System.Text;
using Alerts.Errors;
using Core.Binarize.Exceptions;
using Core.IO;
using Alerts.Warnings;
using Core.Compression;
using Core.Parsing;
using Enumerations;
using Extensions;
using FResults;
using FResults.Extensions;
using FResults.Reasoning;
using Microsoft.Extensions.Logging;
using Options;
using Stubs;

public interface IRVBankDataEntry : IRVBankEntry
{
    RVBankDataType PackingMethod { get; set; }
    void ExpandDirectoryStructure();
}

public class RVBankDataEntry : RVBankEntry, IRVBankDataEntry
{

    private RVBankDataType packingMethod;
    public RVBankDataType PackingMethod
    {
        get => packingMethod;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            packingMethod = value;
        }
    }

    private RVBankDataType AssumePackingMethod()
    {
        switch (EntryMime)
        {
            case RVBankEntryMime.Encrypted:
                return RVBankDataType.Encrypted;
            case RVBankEntryMime.Decompressed:
                return RVBankDataType.Original;
            default:
                if (OriginalSize == 0 || OriginalSize == DataSize)
                {
                    return RVBankDataType.Original;
                }

                return RVBankDataType.Compressed;
        }
    }


    public RVBankDataEntry(BisBinaryReader reader, RVBankOptions options, IRVBank file, IRVBankDirectory parent, ILogger? logger) : base(reader, options, file, parent, logger)
    {
        Debinarize(reader, options);
        if (LastResult!.IsFailed)
        {
            throw new DebinarizeFailedException(LastResult.ToString());
        }
    }

    public void ExpandDirectoryStructure()
    {
        ArgumentNullException.ThrowIfNull(BankFile, "When expanding a Pbo Entry, The node must be established");

        var normalizePath = EntryName = RVPathUtilities.NormalizePboPath(EntryName);

        if (!EntryName.Contains('\\'))
        {
            return;
        }
        EntryName = RVPathUtilities.GetFilename(EntryName);

        Move(BankFile.GetOrCreateDirectory(RVPathUtilities.GetParent(normalizePath), BankFile, Logger));
    }

    public sealed override Result Binarize(BisBinaryWriter writer, RVBankOptions options)
    {
        writer.WriteAsciiZ(Path, options);
        writer.Write((uint)EntryMime);
        writer.Write(OriginalSize);
        writer.Write(Offset);
        writer.Write(TimeStamp);
        writer.Write(DataSize);
        return LastResult = Result.Ok();
    }


    public sealed override Result Validate(RVBankOptions options)
    {
        LastResult = Result.Ok();

        switch (EntryMime)
        {
            case RVBankEntryMime.Encrypted:
                LastResult.WithWarning(new RVBankEncryptedEntryWarning(!options.AllowEncrypted));
                break;
            case RVBankEntryMime.Version:
                LastResult.WithWarning(new RVBankImproperMimeWarning(!options.AllowVersionMimeOnData,
                    typeof(IRVBankDataEntry)));
                break;
            case RVBankEntryMime.Decompressed:
            case RVBankEntryMime.Compressed:
            default:
                break;
        }

        if (EntryName.Length == 0)
        {
            LastResult.WithWarning(new RVBankUnnamedEntryWarning(!options.AllowUnnamedDataEntries, typeof(IRVBankDataEntry)));
        }

        // if (EntryData.Length != DataSize && options.CurrentSection != RVBankSection.Header)
        // {
        //     LastResult.WithWarning(new Warning
        //     {
        //         AlertScope = typeof(IRVBankDataEntry),
        //         AlertName = "EntryReadError",
        //         Message = "Incorrect Stream/DataSize Value.",
        //         IsError = !options.IgnoreInvalidStreamSize
        //     });
        // }

        if (DataSize <= 0)
        {
            LastResult.WithWarning(new Warning
            {
                AlertScope = typeof(IRVBankDataEntry),
                AlertName = "EntryReadError",
                Message = "Incorrect DataSize Value.",
                IsError = !options.IgnoreInvalidStreamSize
            });
        }

        if (OriginalSize <= 0)
        {
            LastResult.WithWarning(new Warning
            {
                AlertScope = typeof(IRVBankDataEntry),
                AlertName = "EntryReadError",
                Message = "Incorrect OriginalSize Value.",
                IsError = !options.IgnoreInvalidStreamSize
            });
        }

        return LastResult;
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RVBankOptions options) => base.Debinarize(reader, options);

    public sealed override int CalculateHeaderLength(RVBankOptions options) => base.CalculateHeaderLength(options);
}
