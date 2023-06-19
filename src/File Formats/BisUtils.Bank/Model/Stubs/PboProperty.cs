namespace BisUtils.Bank.Model.Stubs;

using System.Diagnostics;
using Alerts.Errors;
using Core.Binarize.Exceptions;
using Core.Cloning;
using Core.Family;
using Core.IO;
using Entry;
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
#if DEBUG
        var watch = Stopwatch.StartNew();
#endif

        var result = reader.ReadAsciiZ(out name, options);
        LastResult = name.Length == 0
            ? Result.Fail(PboEmptyPropertyNameError.Instance)
            : Result.Merge(result, reader.ReadAsciiZ(out value, options), Validate(options));
#if DEBUG
        watch.Stop();
        Console.WriteLine($"(PboProperty::Debinarize) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif

        return LastResult;
    }

    public override Result Validate(PboOptions options)
    {
#if DEBUG
        var watch = Stopwatch.StartNew();
#endif
        LastResult = Result.FailIf(Name.Length == 0 || Value.Length == 0, PboEmptyPropertyNameError.Instance);
#if DEBUG
        watch.Stop();
        Console.WriteLine($"(PboProperty::Validate) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif

        return LastResult;
    }

    public IPboProperty BisClone() => new PboProperty(PboFile, VersionEntry, name, value);
}
