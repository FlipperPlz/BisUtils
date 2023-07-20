namespace BisUtils.RVBank.Alerts.Warnings;

using FResults.Reasoning;
using Model.Stubs;

public class RVBankImproperMetaWarning : WarningBase
{
    public RVBankImproperMetaWarning(bool isError = true, Type? type = null)
    {
        IsError = isError;
        AlertScope = type ?? typeof(RVBankEntry);
    }

    public override string? AlertName
    {
        get => "ImproperMeta";
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
        get => "An improper meta was found on an entry";
        set => throw new NotSupportedException();
    }
}
