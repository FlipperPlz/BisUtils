namespace BisUtils.RVBank.Model.Stubs;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.IO;
using Options;

public interface IRVBankElement : IStrictBinaryObject<RVBankOptions>
{
    IRVBank BankFile { get; }
}

public abstract class RVBankElement : StrictBinaryObject<RVBankOptions>, IRVBankElement
{
    public IRVBank BankFile { get; set; } = null!;

    protected RVBankElement()
    {

    }

    protected RVBankElement(IRVBank file) : base() => BankFile = file;

    protected RVBankElement(IRVBank file, BisBinaryReader reader, RVBankOptions options) : base(reader, options) =>
        BankFile = file;
}
