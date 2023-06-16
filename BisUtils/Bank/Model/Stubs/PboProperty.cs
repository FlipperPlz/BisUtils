namespace BisUtils.Bank.Model.Stubs;

using Alerts.Errors;
using Core.Binarize.Exceptions;
using Core.Cloning;
using Core.Family;
using Core.IO;
using Entry;
using FResults;

public interface IPboProperty : IFamilyChild, IPboElement, IBisCloneable<IPboProperty>
{
    public PboVersionEntry? VersionEntry { get; }
    public string Name { get; }
    public string Value { get; }
}

public class PboProperty : PboElement, IPboProperty
{
    public IFamilyParent? Parent => VersionEntry;
    public PboVersionEntry? VersionEntry { get; set; }
    private string name = string.Empty, value = string.Empty;
    public string Name
    {
        get => name;
        set => name = value;
    }

    public string Value
    {
        get => value;
        set => this.value = value;
    }

    public PboProperty(string name, string value) : base()
    {
        this.name = name;
        this.value = value;
    }

    public PboProperty(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
        var result = Debinarize(reader, options);
        if (result.IsFailed)
        {
            throw new DebinarizeFailedException(result.ToString());
        }
    }

    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        writer.WriteAsciiZ(name, options);
        writer.WriteAsciiZ(value, options);
        return Result.ImmutableOk();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, PboOptions options) =>
        Result.Merge(new[] { reader.ReadAsciiZ(out name, options), reader.ReadAsciiZ(out value, options) });

    public override Result Validate(PboOptions options) =>
        Result.FailIf(Name.Length == 0 || Value.Length == 0, PboEmptyPropertyNameError.Instance);

    public IPboProperty BisClone() => new PboProperty(name, value) { VersionEntry = VersionEntry };
}
