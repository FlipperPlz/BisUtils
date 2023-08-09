namespace BisUtils.RvBank.Model.Stubs;

using Core.Binarize.Synchronization;
using Core.IO;
using Core.Logging;
using Microsoft.Extensions.Logging;
using Options;

public interface IRvBankElement : IBisLoggable, IBisSynchronizableElement<RvBankOptions>
{
    IRvBank BankFile { get; }
}

public abstract class RvBankElement : BisSynchronizableElement<RvBankOptions>, IRvBankElement
{
    public IRvBank BankFile { get; set; }
    public bool IsFirstRead { get; private set; }

    protected RvBankElement(IRvBank file, ILogger? logger) : base(file, logger) =>
        BankFile = file;

    protected RvBankElement(BisBinaryReader reader, RvBankOptions options, IRvBank file, ILogger? logger) : base(reader, options, file, logger) =>
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
