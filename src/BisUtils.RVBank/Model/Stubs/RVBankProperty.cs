namespace BisUtils.RVBank.Model.Stubs;

using Core.Cloning;
using Core.IO;
using Alerts.Errors;
using Core.Extensions;
using Entry;
using FResults;
using FResults.Extensions;
using Microsoft.Extensions.Logging;
using Options;

public interface IRVBankProperty : IRVBankElement, IBisCloneable<IRVBankProperty>
{
    IRVBankVersionEntry VersionEntry { get; }
    string Name { get; set; }
    string Value { get; set; }
}

public class RVBankProperty : RVBankElement, IRVBankProperty
{
    public IRVBankVersionEntry VersionEntry { get; private set; }

    private string name = null!;
    public string Name
    {
        get => name;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            name = value;
        }
    }

    private string value = null!;
    public string Value
    {
        get => value;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            this.value = value;
        }
    }

    public RVBankProperty(string name, string value, IRVBank file, IRVBankVersionEntry parent, ILogger? logger) : base(file, logger)
    {
        VersionEntry = parent;
        VersionEntry.ChangesSaved += OnChangesSaved;
        parent.MonitorElement(this);

        Name = name;
        Value = value;
    }

    public RVBankProperty(BisBinaryReader reader, RVBankOptions options, IRVBank file, IRVBankVersionEntry parent, ILogger? logger) : base(reader, options, file, logger)
    {
        VersionEntry = parent;
        VersionEntry.ChangesSaved += OnChangesSaved;
        VersionEntry.MonitorElement(this);

        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    ~RVBankProperty() => VersionEntry.IgnoreElement(this);

    public override Result Binarize(BisBinaryWriter writer, RVBankOptions options)
    {
        writer.WriteAsciiZ(Name, options);
        writer.WriteAsciiZ(Value, options);
        return LastResult = Result.Ok();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RVBankOptions options)
    {
        LastResult = reader.ReadAsciiZ(out name, options);
        if (Name.Length == 0)
        {
            return LastResult.WithError(RVBankEmptyPropertyNameError.Instance);
        }

        LastResult.WithReasons(reader.ReadAsciiZ(out value, options).Reasons);

        return LastResult;
    }

    public override Result Validate(RVBankOptions options)
    {
        LastResult = Result.FailIf(Name.Length == 0 || Value.Length == 0, RVBankEmptyPropertyNameError.Instance);

        return LastResult;
    }

    public IRVBankProperty BisClone() => VersionEntry.CreateVersionProperty(Name, Value);
}
