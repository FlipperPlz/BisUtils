namespace BisUtils.RVBank.Alerts.Warnings;

using FResults.Reasoning;
using Model.Stubs;

public class RVBankNamedVersionEntryWarning : WarningBase
{
    public RVBankNamedVersionEntryWarning(bool isError = true, Type? type = null)
    {
        IsError = isError;
        AlertScope = type ?? typeof(RVBankEntry);
    }

    public override string? AlertName
    {
        get => "NamedVersionEntry";
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
        get => "Version entries are supposed to have an empty name.";
        set => throw new NotSupportedException();
    }
}
