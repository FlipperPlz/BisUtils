using BisUtils.Bank.Model.Stubs;
using BisUtils.Core.Family;
using BisUtils.Core.IO;

namespace BisUtils.Bank.Model;

using Core.Binarize.Exceptions;
using FResults;

public interface IPboFile : IPboDirectory, IFamilyNode
{
    PboFile? Bank { get; }

    IFamilyNode? IFamilyMember.Node => Bank;
    IEnumerable<IFamilyMember> IFamilyParent.Children => PboEntries;
}

public class PboFile : PboDirectory, IPboFile
{
    public PboFile Bank => this;

    public PboFile(List<PboEntry> children) : base(children, "prefix") //TODO: Identify prefix overwrite path and absolutepath
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
        //TODO: Read PBO
        return Result.ImmutableOk();
    }

    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        var responses = new List<Result> { base.Binarize(writer, options) };
        //TODO: Write data and signature

        return Result.Merge(responses);
    }

    public override Result Validate(PboOptions options) =>
        Result.Merge(new[]
        {
            base.Validate(options)
            //TODO: File level validation
        });

}
