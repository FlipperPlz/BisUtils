namespace BisUtils.Bank.Model.Stubs;

using Core.Binarize.Utils;
using Core.Family;
using Core.IO;
using Entry;

public class PboProperty : PboElement, IFamilyChild
{
    public IFamilyParent? Parent => VersionEntry;
    public PboVersionEntry? VersionEntry { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }


    public PboProperty(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public override BinarizationResult Binarize(BisBinaryWriter writer, PboOptions options)
    {
        writer.WriteAsciiZ(Name, options);
        writer.WriteAsciiZ(Value, options);
        return BinarizationResult.Okay;
    }

    public override BinarizationResult Debinarize(BisBinaryReader reader, PboOptions options)
    {
        var result = reader.ReadAsciiZ(out var name, options);

        if (result.IsNotValid)
        {
            return result;
        }
        Name = name;

        return reader.ReadAsciiZ(out var value, options);
    }

    public override bool Validate(PboOptions options) => Name.Length != 0 && Value.Length != 0;
}
