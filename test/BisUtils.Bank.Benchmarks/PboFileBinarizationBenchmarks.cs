namespace BisUtils.Bank.Benchmarks;

using BenchmarkDotNet.Attributes;
using Core.IO;
using FResults;
using Microsoft.Extensions.Logging;
using RVBank.Model;

public class PboFileBinarizationBenchmarks
{
    private readonly RVBank pbo;
    private readonly BisBinaryWriter destination;

    public PboFileBinarizationBenchmarks(ILogger logger)
    {
        using var fs = File.OpenRead(@"C:\Steam\steamapps\common\DayZ\Addons\weapons_data.pbo");

        pbo = new RVBank("weapons_data", fs, new(), null, logger);

        destination = new BisBinaryWriter(new MemoryStream((int)fs.Length));
    }

    [IterationSetup(Target = nameof(Binarize))]
    public void DebinarizeSetup() => destination.Seek(0, SeekOrigin.Begin);

    [Benchmark]
    public Result Binarize() => pbo.Binarize(destination, new());

    [GlobalCleanup]
    public void Cleanup() => destination.Close();
}
