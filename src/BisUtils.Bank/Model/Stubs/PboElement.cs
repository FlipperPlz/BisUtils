namespace BisUtils.Bank.Model.Stubs;

using BisUtils.Core.Binarize.Implementation;
using BisUtils.Core.Family;
using BisUtils.Core.IO;
using BisUtils.Core.Binarize;
using Options;

public interface IPboElement : IFamilyMember, IStrictBinaryObject<PboOptions>
{
    IPboFile? PboFile { get; }
    IFamilyNode? IFamilyMember.Node => PboFile;
}

public abstract class PboElement : StrictBinaryObject<PboOptions>, IPboElement
{
    public IFamilyNode? Node => PboFile;
    public IPboFile? PboFile { get; set; }

    protected PboElement(IPboFile? file) : base()
    {
    }

    protected PboElement(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {

    }

}
