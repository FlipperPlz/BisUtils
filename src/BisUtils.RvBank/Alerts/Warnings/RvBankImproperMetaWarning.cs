namespace BisUtils.RvBank.Alerts.Warnings;

using FResults.Reasoning;
using Model.Stubs;

public class RvBankImproperMetaWarning : WarningBase
{
    public RvBankImproperMetaWarning(bool isError = true, Type? type = null)
    {
        IsError = isError;
        AlertScope = type ?? typeof(RvBankEntry);
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
