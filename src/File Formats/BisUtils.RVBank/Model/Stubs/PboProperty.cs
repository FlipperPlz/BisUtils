namespace BisUtils.RVBank.Model.Stubs;

using BisUtils.Core.Binarize.Exceptions;
using BisUtils.Core.Cloning;
using BisUtils.Core.Family;
using BisUtils.Core.IO;
using BisUtils.RVBank.Alerts.Errors;
using BisUtils.RVBank.Model.Entry;
using FResults;
using Options;

public interface IPboProperty : IFamilyChild, IPboElement, IBisCloneable<IPboProperty>
{
    IPboVersionEntry? VersionEntry { get; }
    string Name { get; }
    string Value { get; }
}

public class PboProperty : PboElement, IPboProperty
{
    private string name = string.Empty, value = string.Empty;

    public PboProperty(IPboFile? file, IPboVersionEntry? parent, string name, string value) : base(file)
    {
        VersionEntry = parent;
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

    public IFamilyParent? Parent => VersionEntry;
    public IPboVersionEntry? VersionEntry { get; set; }

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

    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        writer.WriteAsciiZ(name, options);
        writer.WriteAsciiZ(value, options);
        return LastResult = Result.ImmutableOk();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, PboOptions options)
    {
        var result = reader.ReadAsciiZ(out name, options);
        LastResult = name.Length == 0
            ? Result.Fail(PboEmptyPropertyNameError.Instance)
            : Result.Merge(result, reader.ReadAsciiZ(out value, options), Validate(options));

        return LastResult;
    }

    public override Result Validate(PboOptions options)
    {
        LastResult = Result.FailIf(Name.Length == 0 || Value.Length == 0, PboEmptyPropertyNameError.Instance);

        return LastResult;
    }

    public IPboProperty BisClone() => new PboProperty(PboFile, VersionEntry, name, value);
}
