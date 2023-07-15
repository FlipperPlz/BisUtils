﻿namespace BisUtils.RVBank.Alerts.Warnings;

using FResults.Reasoning;
using Model.Stubs;

public class PboUnnamedEntryWarning : WarningBase
{
    public PboUnnamedEntryWarning(bool isError = true, Type? type = null)
    {
        IsError = isError;
        AlertScope = type ?? typeof(PboEntry);
    }

    public override string? AlertName
    {
        get => "UnnamedEntry";
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
        get => "A pbo entry was found with no name.";
        set => throw new NotSupportedException();
    }
}
