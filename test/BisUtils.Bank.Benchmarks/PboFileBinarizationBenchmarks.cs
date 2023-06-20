namespace BisUtils.Bank.Benchmarks;

using BenchmarkDotNet.Attributes;
using BisUtils.Bank.Model;
using BisUtils.Core.IO;
using FResults;

public class PboFileBinarizationBenchmarks
{
    private readonly PboFile pbo;
    private readonly BisBinaryWriter destination;

    public PboFileBinarizationBenchmarks()
    {
        using var fs = File.OpenRead(@"C:\Steam\steamapps\common\DayZ\Addons\weapons_data.pbo");
        using var reader = new BisBinaryReader(fs);

        pbo = new PboFile(reader, new());

        destination = new BisBinaryWriter(new MemoryStream((int)fs.Length));
    }

    [IterationSetup(Target = nameof(Binarize))]
    public void DebinarizeSetup() => destination.Seek(0, SeekOrigin.Begin);

    [Benchmark]
    public Result Binarize() => pbo.Binarize(destination, new());

    [GlobalCleanup]
    public void Cleanup() => destination.Close();
}
