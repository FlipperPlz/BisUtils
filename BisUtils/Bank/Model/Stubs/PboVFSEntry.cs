using BisUtils.Core.IO;

namespace BisUtils.Bank.Model.Stubs;

public abstract class PboVFSEntry : PboElement
{
    //TODO: Path & AbsolutePath properties
    
    protected PboVFSEntry() : base()
    {
    }
    
    protected PboVFSEntry(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
    }
}