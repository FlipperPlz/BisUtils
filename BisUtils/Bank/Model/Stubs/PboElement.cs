using BisUtils.Core.Binarize.Implementation;
using BisUtils.Core.Family;
using BisUtils.Core.IO;

namespace BisUtils.Bank.Model.Stubs;

public abstract class PboElement : StrictBinaryObject<PboOptions>, IFamilyMember
{
    public PboFile? PboFile { get; set; }
    public IFamilyNode? Node => PboFile;
    
    protected PboElement() : base()
    {
    }

    protected PboElement(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
        
    }

}