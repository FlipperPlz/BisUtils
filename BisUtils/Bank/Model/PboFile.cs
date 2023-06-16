namespace BisUtils.Bank.Model;

using BisUtils.Bank.Model.Entry;
using BisUtils.Bank.Model.Stubs;
using BisUtils.Core.Family;
using BisUtils.Core.IO;
using BisUtils.Core.Binarize.Exceptions;
using FResults;
using FResults.Reasoning;

public interface IPboFile : IPboDirectory, IFamilyNode
{
    IFamilyNode? IFamilyMember.Node => PboFile;
    IEnumerable<IFamilyMember> IFamilyParent.Children => PboEntries;
}

public class PboFile : PboDirectory, IPboFile
{
    public PboFile(List<PboEntry> children) : base(null, null, children, "prefix") //TODO: Identify prefix overwrite path and absolutepath
    {
    }

    public PboFile(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
        var result = Debinarize(reader, options);
        if (result.IsFailed)
        {
            throw new DebinarizeFailedException(result.ToString());
        }
    }

    public sealed override Result Debinarize(BisBinaryReader reader, PboOptions options)
    {
        var responses = new List<Result>();
        var first = true;

        do
        {
            var start = reader.BaseStream.Position;
            responses.Add(reader.SkipAsciiZ(options));
            var mime = (PboEntryMime?) reader.ReadInt64();
            reader.BaseStream.Seek(start, SeekOrigin.Begin);
            PboEntry currentEntry = mime switch
            {
                PboEntryMime.Version => new PboVersionEntry(reader, options) { ParentDirectory = this, PboFile = this },
                _ => new PboDataEntry(reader, options) { ParentDirectory = this, PboFile = this }
            };
            var response = currentEntry.LastResult ?? Result.Fail("Unknown Error Occured");

            if (first && currentEntry is not IPboVersionEntry)
            {
                first = false;
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

            responses.Add(response);
            PboEntries.Add(currentEntry);
        } while (true);
        //INITIALIZE ENTRY DATA
        return Result.Merge(responses);
    }

    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        var responses = new List<Result> { base.Binarize(writer, options) };


        return Result.Merge(responses);
    }

    public override Result Validate(PboOptions options) =>
        Result.Merge(base.Validate(options));

}
