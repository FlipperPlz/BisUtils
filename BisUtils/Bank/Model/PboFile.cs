using BisUtils.Bank.Model.Stubs;
using BisUtils.Core.Binarize.Utils;
using BisUtils.Core.Family;
using BisUtils.Core.IO;

namespace BisUtils.Bank.Model;

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
    }

    public override BinarizationResult Debinarize(BisBinaryReader reader, PboOptions options)
    {
        //TODO: Read PBO
        return BinarizationResult.Okay;
    }

    public override BinarizationResult Binarize(BisBinaryWriter writer, PboOptions options)
    {
        var response = base.Binarize(writer, options);
        if (!response.IsValid) return response;
        //TODO: Write data and signature

        return response;
    }

    public override bool Validate(PboOptions options)
    {
        if(!base.Validate(options)) return false;
        //TODO: File level validation

        return true;
    }
}
