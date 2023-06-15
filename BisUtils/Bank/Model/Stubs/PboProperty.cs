namespace BisUtils.Bank.Model.Stubs;

using Alerts.Errors;
using Core.Family;
using Core.IO;
using Entry;
using FResults;

public class PboProperty : PboElement, IFamilyChild
{
    public IFamilyParent? Parent => VersionEntry;
    public PboVersionEntry? VersionEntry { get; set; }
    private string name, value;
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

    public PboProperty(string name, string value)
    {
        this.name = name;
        this.value = value;
    }

    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        writer.WriteAsciiZ(name, options);
        writer.WriteAsciiZ(value, options);
        return Result.ImmutableOk();
    }

    public override Result Debinarize(BisBinaryReader reader, PboOptions options) =>
        Result.Merge(new[] { reader.ReadAsciiZ(out name, options), reader.ReadAsciiZ(out value, options) });

    public override Result Validate(PboOptions options) =>
        Result.FailIf(Name.Length == 0 || Value.Length == 0, PboEmptyPropertyNameError.Instance);
}
