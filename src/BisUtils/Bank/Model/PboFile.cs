namespace BisUtils.Bank.Model;

using BisUtils.Bank.Model.Entry;
using BisUtils.Bank.Model.Stubs;
using BisUtils.Core.Family;
using BisUtils.Core.IO;
using BisUtils.Core.Binarize.Exceptions;
using FResults;
using FResults.Extensions;
using FResults.Reasoning;

public interface IPboFile : IPboDirectory, IFamilyNode
{
    IFamilyNode? IFamilyMember.Node => PboFile;
    IEnumerable<IFamilyMember> IFamilyParent.Children => PboEntries;
}


public class PboFile : PboDirectory, IPboFile
{
    public PboFile(List<IPboEntry> children) : base(null, null, children, "prefix") //TODO: Identify prefix overwrite path and absolutepath
    {
    }

    public PboFile(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
        Debinarize(reader, options);
        if (LastResult!.IsFailed)
        {
            throw new DebinarizeFailedException(LastResult.ToString());
        }
    }

    public sealed override Result Debinarize(BisBinaryReader reader, PboOptions options)
    {
        var responses = new List<Result>();
        var first = true;
        options.CurrentSection = PboSection.Header;

        do
        {
            var start = reader.BaseStream.Position;
            responses.Add(reader.SkipAsciiZ(options));
            var mime = (PboEntryMime?) reader.ReadInt64();
            reader.BaseStream.Seek(start, SeekOrigin.Begin);
            IPboEntry currentEntry = mime switch
            {
                PboEntryMime.Version => new PboVersionEntry(reader, options) { ParentDirectory = this, PboFile = this },
                _ => new PboDataEntry(reader, options) { ParentDirectory = this, PboFile = this }
            };

            if (!options.FlatRead && currentEntry is PboDataEntry dataEntry)
            {
                dataEntry.ExpandDirectoryStructure();
            }

            var response = currentEntry.LastResult ?? Result.Fail("Unknown Error Occured");

            if (first && currentEntry is not IPboVersionEntry)
            {
                response.WithWarning(new Warning
                {
                    Message = "The first entry in a PBO should always be a version entry.",
                    AlertName = "FirstIsNotVersion",
                    AlertScope = typeof(PboFile),
                    IsError = options.RequireFirstEntryIsVersion
                });
            }

            if (currentEntry is IPboDataEntry && currentEntry.IsDummyEntry())
            {
                if(options.AlwaysSeparateOnDummy)
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
            PboEntries.Add(currentEntry);
        } while (true);

        options.CurrentSection = PboSection.Data;
        //INITIALIZE ENTRY DATA

        options.CurrentSection = PboSection.Signature;

        options.CurrentSection = PboSection.Finished;
        return Result.Merge(responses);
    }

    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        var result = PboEntries.Select(e => e.Binarize(writer, options));

        return LastResult = Result.Merge(result);
    }

    public override Result Validate(PboOptions options) =>
        Result.Merge(base.Validate(options));

}
