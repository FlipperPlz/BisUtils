namespace BisUtils.Bank.Benchmarks;

using BenchmarkDotNet.Attributes;
using BisUtils.Core.IO;
using RVBank.Model;
using RVBank.Options;

public class PboFileCreationBenchmarks
{
    private readonly BisBinaryReader reader;
    private readonly PboOptions flatReadOptions = new() { FlatRead = true };
    private readonly PboOptions options = new() { FlatRead = false };

    public PboFileCreationBenchmarks()
    {
        var fs = File.OpenRead(@"C:\Steam\steamapps\common\DayZ\Addons\structures_data.pbo");
        reader = new BisBinaryReader(fs);
    }

    [IterationSetup(Target = nameof(DebinarizeFlatRead))]
    public void DebinarizeFlatReadSetup() => reader.BaseStream.Position = 0;

    [Benchmark(Baseline = true)]
    public PboFile DebinarizeFlatRead() => new(reader, flatReadOptions);

    [IterationSetup(Target = nameof(Debinarize))]
    public void DebinarizeSetup() => reader.BaseStream.Position = 0;

    [Benchmark]
    public PboFile Debinarize() => new(reader, options);

    [GlobalCleanup]
    public void Cleanup() => reader.Close();
}
