namespace BisUtils.Bank.Alerts.Warnings;

using BisUtils.Bank.Model.Stubs;
using FResults.Reasoning;

public class PboImproperMetaWarning : WarningBase
{

    public PboImproperMetaWarning(bool isError = true, Type? type = null)
    {
        IsError = isError;
        AlertScope = type ?? typeof(PboEntry);
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
