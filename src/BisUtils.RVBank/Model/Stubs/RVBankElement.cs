namespace BisUtils.RVBank.Model.Stubs;

using Core.Binarize.Synchronization;
using Core.IO;
using Core.Logging;
using Microsoft.Extensions.Logging;
using Options;

public interface IRVBankElement : IBisLoggable, IBisSynchronizableElement<RVBankOptions>
{
    IRVBank BankFile { get; }
}

public abstract class RVBankElement : BisSynchronizableElement<RVBankOptions>, IRVBankElement
{
    public IRVBank BankFile { get; set; }
    public bool IsFirstRead { get; private set; }

    protected RVBankElement(IRVBank file, ILogger? logger) : base(file, logger) =>
        BankFile = file;

    protected RVBankElement(BisBinaryReader reader, RVBankOptions options, IRVBank file, ILogger? logger) : base(reader, options, file, logger) =>
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
