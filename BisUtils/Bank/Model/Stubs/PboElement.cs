using BisUtils.Core.Binarize.Implementation;
using BisUtils.Core.Family;
using BisUtils.Core.IO;

namespace BisUtils.Bank.Model.Stubs;

using Core.Binarize;

public interface IPboElement : IFamilyMember, IStrictBinaryObject<PboOptions>
{
    IPboFile? PboFile { get; }
    IFamilyNode? IFamilyMember.Node => PboFile;
}

public abstract class PboElement : StrictBinaryObject<PboOptions>, IFamilyMember
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
