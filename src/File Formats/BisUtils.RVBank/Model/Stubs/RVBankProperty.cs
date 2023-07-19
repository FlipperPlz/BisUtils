namespace BisUtils.RVBank.Model.Stubs;

using Core.Cloning;
using Core.IO;
using Alerts.Errors;
using Core.Extensions;
using Entry;
using FResults;
using FResults.Extensions;
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
            OnChangesMade(EventArgs.Empty);
            name = value;
        }
    }

    private string value = null!;
    public string Value
    {
        get => value;
        set
        {
            OnChangesMade(EventArgs.Empty);
            this.value = value;
        }
    }

    public RVBankProperty(IRVBank file, IRVBankVersionEntry parent, string name, string value) : base(file)
    {
        VersionEntry = parent;
        Name = name;
        Value = value;
    }

    public RVBankProperty(IRVBank file, IRVBankVersionEntry parent, BisBinaryReader reader, RVBankOptions options) : base(file, reader, options)
    {
        VersionEntry = parent;
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

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

    public IRVBankProperty BisClone() => new RVBankProperty(BankFile, VersionEntry, Name, Value);
}