namespace BisUtils.RvBank.Alerts.Warnings;

using FResults.Reasoning;
using Model.Entry;

public class RvBankEncryptedEntryWarning : WarningBase
{
    public RvBankEncryptedEntryWarning(bool isError = true)
    {
        IsError = isError;
        AlertScope = typeof(RvBankDataEntry);
    }

    public override string? AlertName
    {
        get => "EncryptedEntryMime";
        init => throw new NotSupportedException();
    }

    public sealed override bool IsError
    {
        get;
        set;
    }

    public sealed override Type? AlertScope
    {
        get;
        init;
    }

    public override string? Message
    {
        get => "An entry was found with an encrypted mime. No decryption algorithm present. ";
        set => throw new NotSupportedException();
    }
}
