namespace BisUtils.RVBank.Model;
using Core.Extensions;
using Core.IO;
using Enumerations;
using Entry;
using Stubs;
using FResults;
using FResults.Extensions;
using FResults.Reasoning;
using Options;

public interface IRVBank : IRVBankDirectory
{
}

public class RVBank : RVBankDirectory, IRVBank
{
    public RVBank(List<IRVBankEntry> children) :
        base(null!, null!, children, "prefix") //TODO: Identify prefix overwrite path and absolutepath
    {
    }

    public RVBank(BisBinaryReader reader, RVBankOptions options) : base(null!, null!, reader, options)
    {
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
                RVBankEntryMime.Version => new RVBankVersionEntry(this, reader, options) { ParentDirectory = this, BankFile = this },
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

    public override Result Validate(RVBankOptions options) =>
        Result.Merge(base.Validate(options));
}
