using BisUtils.Bank.Model.Stubs;
using BisUtils.Core.Family;
using BisUtils.Core.IO;

namespace BisUtils.Bank.Model;

using Core.Binarize.Exceptions;
using FResults;

public class PboFile : PboDirectory, IFamilyNode
{
    public new PboFile? Node => this;
    public new readonly List<PboEntry> PboEntries = new();
    public new IEnumerable<IFamilyMember> Children => PboEntries;

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

    public override Result Debinarize(BisBinaryReader reader, PboOptions options)
    {
        //TODO: Read PBO
        return Result.Ok();
    }

    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        var response = base.Binarize(writer, options);
        if (response.IsFailed)
        {
            return response;
        }
        //TODO: Write data and signature

        return response;
    }

    public override Result Validate(PboOptions options)
    {
        if (!base.Validate(options))
        {
            return Result.Fail("");
        }
        //TODO: File level validation

        return Result.Ok();

    }
}
