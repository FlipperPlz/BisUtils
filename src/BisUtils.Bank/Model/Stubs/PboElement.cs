namespace BisUtils.Bank.Model.Stubs;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Family;
using Core.IO;
using Options;

public interface IPboElement : IFamilyMember, IStrictBinaryObject<PboOptions>
{
    IPboFile? PboFile { get; }
    IFamilyNode? IFamilyMember.Node => PboFile;
}

public abstract class PboElement : StrictBinaryObject<PboOptions>, IPboElement
{
    protected PboElement(IPboFile? file) : base()
    {
    }

    protected PboElement(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
    }

    public IFamilyNode? Node => PboFile;
    public IPboFile? PboFile { get; set; }
}
