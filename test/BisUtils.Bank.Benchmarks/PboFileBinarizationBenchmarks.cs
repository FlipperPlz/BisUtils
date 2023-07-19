namespace BisUtils.Bank.Benchmarks;

using BenchmarkDotNet.Attributes;
using BisUtils.Core.IO;
using FResults;
using RVBank.Model;

public class PboFileBinarizationBenchmarks
{
    private readonly RVBank pbo;
    private readonly BisBinaryWriter destination;

    public PboFileBinarizationBenchmarks()
    {
        using var fs = File.OpenRead(@"C:\Steam\steamapps\common\DayZ\Addons\weapons_data.pbo");
        using var reader = new BisBinaryReader(fs);

        pbo = new RVBank("weapons_data", reader, new());

        destination = new BisBinaryWriter(new MemoryStream((int)fs.Length));
    }

    [IterationSetup(Target = nameof(Binarize))]
    public void DebinarizeSetup() => destination.Seek(0, SeekOrigin.Begin);

    [Benchmark]
    public Result Binarize() => pbo.Binarize(destination, new());

    [GlobalCleanup]
    public void Cleanup() => destination.Close();
}
