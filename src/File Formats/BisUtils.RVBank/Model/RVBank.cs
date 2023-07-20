namespace BisUtils.RVBank.Model;

using System.Collections.ObjectModel;
using Core.Binarize.Synchronization;
using Core.Extensions;
using Core.IO;
using Enumerations;
using Entry;
using Extensions;
using Stubs;
using FResults;
using FResults.Extensions;
using FResults.Reasoning;
using Options;

public interface IRVBank : IBisSynchronizable<RVBankOptions>, IRVBankDirectory
{
    public string FileName { get; set; }

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
    public string FileName { get; set; }

    public string BankPrefix
    {
        get => this.GetVersionEntry()?.GetPropertyValue("prefix") ?? FileName;
        set => (this.GetVersionEntry() ?? this.AddVersionEntry()).SetOrCreateProperty("prefix", value);
    }
    public IRVBank BankFile { get; }

    public bool IsFirstRead { get; private set; } = true;

    private readonly ObservableCollection<IRVBankEntry> pboEntries = null!;
    public ObservableCollection<IRVBankEntry> PboEntries
    {
        get => pboEntries;
        init
        {
            pboEntries = value;
            pboEntries.CollectionChanged += (_, args) =>
            {
                OnChangesMade(this, args);
            };
        }
    }

    public RVBank(string filename, IEnumerable<IRVBankEntry> entries, Stream? synchronizeTo = null) : base(synchronizeTo)
    {
        BankFile = this;
        FileName = filename;
        PboEntries = new ObservableCollection<IRVBankEntry>(entries);
    }

    public RVBank(string filename, BisBinaryReader reader, RVBankOptions options, Stream? synchronizeTo = null) : base(reader, options, synchronizeTo)
    {
        BankFile = this;
        FileName = filename;
        PboEntries = new ObservableCollection<IRVBankEntry>();
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RVBankOptions options)
    {
        var responses = new List<Result>();
        var first = true;
        do
        {
            var start = reader.BaseStream.Position;
            responses.Add(reader.SkipAsciiZ(options));
            var mime = (RVBankEntryMime?)reader.ReadInt64();
            reader.BaseStream.Seek(start, SeekOrigin.Begin);
            IRVBankEntry currentEntry = mime switch
            {
                RVBankEntryMime.Version => new RVBankVersionEntry(this, this, reader, options),
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

        LastResult = Result.Merge(responses);
        if (reader.BaseStream == SynchronizationStream)
        {
            OnChangesSaved(EventArgs.Empty);
        }

        if (IsFirstRead)
        {
            IsFirstRead = false;
        }

        return LastResult;
    }

    public override Result Binarize(BisBinaryWriter writer, RVBankOptions options)
    {

        throw new NotImplementedException();
        return LastResult = Result.Ok().WithReasons(PboEntries.SelectMany(e => e.Binarize(writer, options).Reasons));
    }

    public override Result Validate(RVBankOptions options) => throw new NotImplementedException();

    public void Move(IRVBankDirectory destination) => throw new NotSupportedException();
}
