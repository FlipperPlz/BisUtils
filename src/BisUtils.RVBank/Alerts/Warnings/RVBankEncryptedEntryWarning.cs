﻿namespace BisUtils.RVBank.Alerts.Warnings;

using FResults.Reasoning;
using Model.Entry;

public class RVBankEncryptedEntryWarning : WarningBase
{
    public RVBankEncryptedEntryWarning(bool isError = true)
    {
        IsError = isError;
        AlertScope = typeof(RVBankDataEntry);
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
