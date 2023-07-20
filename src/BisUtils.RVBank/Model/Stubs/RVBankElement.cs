namespace BisUtils.RVBank.Model.Stubs;

using Core.Binarize.Synchronization;
using Core.IO;
using Options;

public interface IRVBankElement : IBisSynchronizableElement<RVBankOptions>
{
    IRVBank BankFile { get; }
}

public abstract class RVBankElement : BisSynchronizableElement<RVBankOptions>, IRVBankElement
{
    public IRVBank BankFile { get; set; }
    public bool IsFirstRead { get; private set; }

    protected RVBankElement(IRVBank file) : base(file) =>
        BankFile = file;

    protected RVBankElement(IRVBank file, BisBinaryReader reader, RVBankOptions options) : base(reader, options, file) =>
        BankFile = file;

    protected override void OnChangesMade(object? sender, EventArgs? e)
    {
        if (IsFirstRead)
        {
            IsFirstRead = false;
            return;
        }
        base.OnChangesMade(sender, e);
    }
}
