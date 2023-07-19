namespace BisUtils.RVBank.Model;

using Core.Binarize.Synchronization;
using Core.Extensions;
using Core.IO;
using Enumerations;
using Entry;
using Stubs;
using FResults;
using FResults.Extensions;
using FResults.Reasoning;
using Options;

public interface IRVBank : IBisSynchronizable<RVBankOptions>, IRVBankDirectory
{
    public string BankPrefix { get; set; }

    IRVBankDirectory IRVBankVfsEntry.ParentDirectory => this;

    string IRVBankVfsEntry.Path => "";

    string IRVBankVfsEntry.AbsolutePath => BankPrefix;

    string IRVBankVfsEntry.EntryName
    {
        get => BankPrefix;
        set => BankPrefix = value;
    }

}

public class RVBank : BisSynchronizable<RVBankOptions>, IRVBank
{
    public string BankPrefix { get; set; }
    public IRVBank BankFile { get; }
    public bool IsFirstRead { get; private set; } = true;
    public List<IRVBankEntry> PboEntries { get; set; }
    public IEnumerable<IRVBankDataEntry> FileEntries => PboEntries.OfType<IRVBankDataEntry>();
    public IEnumerable<IRVBankDirectory> Directories => PboEntries.OfType<IRVBankDirectory>();

    public RVBank(string filename, List<IRVBankEntry> entries, Stream? synchronizeTo = null) : base(synchronizeTo)
    {
        BankFile = this;
        BankPrefix = filename;
        PboEntries = entries;
    }

    public RVBank(string filename, BisBinaryReader reader, RVBankOptions options, Stream? synchronizeTo = null) : base(reader, options, synchronizeTo)
    {
        BankFile = this;
        BankPrefix = filename;
        PboEntries = new List<IRVBankEntry>();
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RVBankOptions options)
    {
        var responses = new List<Result>();
        var first = true;
        options.CurrentSection = RVBankSection.Header;

        do
        {
            var start = reader.BaseStream.Position;
            responses.Add(reader.SkipAsciiZ(options));
            var mime = (RVBankEntryMime?)reader.ReadInt64();
            reader.BaseStream.Seek(start, SeekOrigin.Begin);
            IRVBankEntry currentEntry = mime switch
            {
                RVBankEntryMime.Version => new RVBankVersionEntry(this, reader, options),
                _ => new RVBankDataEntry(this, this, reader, options)
            };

            if (!options.FlatRead && currentEntry is RVBankDataEntry dataEntry)
            {
                dataEntry.ExpandDirectoryStructure();
            }

            var response = currentEntry.LastResult ?? Result.Fail("Unknown Error Occured");

            if (first && currentEntry is not IRVBankVersionEntry)
            {
                response.WithWarning(new Warning
                {
                    Message = "The first entry in a PBO should always be a version entry.",
                    AlertName = "FirstIsNotVersion",
                    AlertScope = typeof(RVBank),
                    IsError = options.RequireFirstEntryIsVersion
                });
            }

            if (currentEntry is IRVBankDataEntry && currentEntry.IsDummyEntry())
            {
                if (options.AlwaysSeparateOnDummy)
                {
                    break;
                }

                throw new NotImplementedException("TODO: Recover On Empty Not Implemented");
            }

            if (first)
            {
                first = false;
            }

            responses.Add(response);
            if (currentEntry.ParentDirectory == this)
            {
                PboEntries.Add(currentEntry);
            }
        } while (true);

        options.CurrentSection = RVBankSection.Data;
        //INITIALIZE ENTRY DATA

        options.CurrentSection = RVBankSection.Signature;

        options.CurrentSection = RVBankSection.Finished;
        LastResult = Result.Merge(responses);

        return LastResult;
    }

    public override Result Binarize(BisBinaryWriter writer, RVBankOptions options)
    {
        var result = PboEntries.Select(e => e.Binarize(writer, options));

        return LastResult = Result.Merge(result);
    }

    public override Result Validate(RVBankOptions options) => throw new NotImplementedException();
}
