namespace BisUtils.RvBank.Model.Stubs;

using Core.Cloning;
using Core.IO;
using Alerts.Errors;
using Core.Extensions;
using Entry;
using Extensions;
using FResults;
using FResults.Extensions;
using Microsoft.Extensions.Logging;
using Options;

public interface IRvBankProperty : IRvBankElement, IBisCloneable<IRvBankProperty>
{
    IRvBankVersionEntry VersionEntry { get; }
    string Name { get; set; }
    string Value { get; set; }
}

public class RvBankProperty : RvBankElement, IRvBankProperty
{
    public IRvBankVersionEntry VersionEntry { get; private set; }

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

    public RvBankProperty(string name, string value, IRvBank file, IRvBankVersionEntry parent, ILogger? logger) : base(file, logger)
    {
        VersionEntry = parent;
        VersionEntry.ChangesSaved += OnChangesSaved;
        parent.MonitorElement(this);

        Name = name;
        Value = value;
    }

    public RvBankProperty(BisBinaryReader reader, RvBankOptions options, IRvBank file, IRvBankVersionEntry parent, ILogger? logger) : base(reader, options, file, logger)
    {
        VersionEntry = parent;
        VersionEntry.ChangesSaved += OnChangesSaved;
        VersionEntry.MonitorElement(this);

        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    ~RvBankProperty() => VersionEntry.IgnoreElement(this);

    public override Result Binarize(BisBinaryWriter writer, RvBankOptions options)
    {
        writer.WriteAsciiZ(Name, options);
        writer.WriteAsciiZ(Value, options);
        return LastResult = Result.Ok();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RvBankOptions options)
    {
        LastResult = reader.ReadAsciiZ(out name, options);
        if (Name.Length == 0)
        {
            return LastResult.WithError(RvBankEmptyPropertyNameError.Instance);
        }

        LastResult.WithReasons(reader.ReadAsciiZ(out value, options).Reasons);

        return LastResult;
    }

    public override Result Validate(RvBankOptions options)
    {
        LastResult = Result.FailIf(Name.Length == 0 || Value.Length == 0, RvBankEmptyPropertyNameError.Instance);

        return LastResult;
    }

    public IRvBankProperty BisClone() => VersionEntry.CreateVersionProperty(Name, Value, Logger);
}
