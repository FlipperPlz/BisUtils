﻿namespace BisUtils.RvBank.Alerts.Warnings;

using FResults.Reasoning;
using Model.Stubs;

public class RvBankNamedVersionEntryWarning : WarningBase
{
    public RvBankNamedVersionEntryWarning(bool isError = true, Type? type = null)
    {
        IsError = isError;
        AlertScope = type ?? typeof(RvBankEntry);
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