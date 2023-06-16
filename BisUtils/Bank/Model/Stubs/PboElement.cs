using BisUtils.Core.Binarize.Implementation;
using BisUtils.Core.Family;
using BisUtils.Core.IO;

namespace BisUtils.Bank.Model.Stubs;

public interface IPboElement : IFamilyMember
{
    public PboFile? PboFile { get; }
    IFamilyNode? IFamilyMember.Node => PboFile;
}

public abstract class PboElement : StrictBinaryObject<PboOptions>, IFamilyMember
{
    public IFamilyNode? Node => PboFile;
    public PboFile? PboFile { get; set; }

    protected PboElement() : base()
    {
    }

    protected PboElement(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {

    }

}
