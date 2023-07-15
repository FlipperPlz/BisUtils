namespace BisUtils.RVBank.Alerts.Warnings;

using FResults.Reasoning;
using Model.Stubs;

public class PboImproperMimeWarning : WarningBase
{
    public PboImproperMimeWarning(bool isError = true, Type? type = null)
    {
        IsError = isError;
        AlertScope = type ?? typeof(PboEntry);
    }

    public override string? AlertName
    {
        get => "ImproperMime";
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
        get => "An improper mime was found on an entry";
        set => throw new NotSupportedException();
    }
}
