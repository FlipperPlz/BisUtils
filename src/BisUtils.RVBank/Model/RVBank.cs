namespace BisUtils.RVBank.Model;

using System.Collections.ObjectModel;
using System.Security.Cryptography;
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
using Misc;
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
        if(pboEntries.Count != 0)
        {
            PboEntries.Clear();
        }

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

        foreach (var entry in this.GetFileEntries())
        {
            entry.InitializeData(reader, options);
        }

        var remaining = reader.BaseStream.Length - reader.BaseStream.Position;

        switch (remaining)
        {
            case < 20:
                //TODO: Log and warn that a full checksum was not found; reading is finished
                goto End;
            case >= 20:
                //TODO: Log and warn that extra data was found
                break;
        }

        var calculatedDigest = CalculateDigest(reader.BaseStream);
        var writtenDigest = ReadDigest(reader);
        if (writtenDigest != calculatedDigest)
        {
            //TODO: Log and warn that invalid checksum was found
        }

        End:
        {
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
    }

    public override Result Binarize(BisBinaryWriter writer, RVBankOptions options)
    {
        var headerStart = writer.BaseStream.Position;
        var headerLength = PboEntries.Sum(it => it.CalculateLength(options));
        //var dataStart = writer.BaseStream.Position;
        //long dataLength = 0;
        writer.BaseStream.Seek(headerLength, SeekOrigin.Current);
        foreach (var entry in this.GetFileEntries(true))
        {
            var data = entry.RetrieveFinalStream(out var compressed);
            //dataLength += data.Length;
            writer.Write(data);
            if (compressed)
            {
                data.Close();
            }
        }
        var dataEnd = writer.BaseStream.Position;
        writer.BaseStream.Seek(headerStart, SeekOrigin.Begin);
        LastResult = Result.Ok().WithReasons(PboEntries.SelectMany(it => it.Binarize(writer, options).Reasons));
        writer.BaseStream.Seek(dataEnd, SeekOrigin.Begin);
        var digest = CalculateDigest(writer.BaseStream);
        digest.Write(writer, options);
        return LastResult;
    }

    public override Result Validate(RVBankOptions options) => throw new NotImplementedException();

    private static RVBankDigest ReadDigest(BinaryReader reader) =>
        new(reader.ReadBytes(20));

#pragma warning disable SYSLIB0021
#pragma warning disable CA5350
    private static RVBankDigest CalculateDigest(Stream stream)
    {
        var oldPosition = stream.Position;
        stream.Seek(0, SeekOrigin.Begin);


        using var alg = new SHA1Managed();
        var digest = new RVBankDigest(alg.ComputeHash(stream));

        stream.Seek(oldPosition, SeekOrigin.Begin);

        return digest;
    }
#pragma warning restore CA5350
#pragma warning restore SYSLIB0021
    public long CalculateLength(RVBankOptions options) => pboEntries.Sum(it => it.CalculateLength(options) + it.DataSize) + 20;
}
