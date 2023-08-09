namespace BisUtils.RvBank.Benchmarks;

using BenchmarkDotNet.Attributes;
using Core.IO;
using FResults;
using Microsoft.Extensions.Logging;
using RvBank.Model;

public class RvBankBinarizationBenchmarks
{
    private readonly RvBank pbo;
    private readonly BisBinaryWriter destination;

    public RvBankBinarizationBenchmarks(ILogger logger)
    {
        using var fs = File.OpenRead(@"C:\Steam\steamapps\common\DayZ\Addons\weapons_data.pbo");

        pbo = new RvBank("weapons_data", fs, new(), null, logger);

        destination = new BisBinaryWriter(new MemoryStream((int)fs.Length));
    }

    [IterationSetup(Target = nameof(Binarize))]
    public void DebinarizeSetup() => destination.Seek(0, SeekOrigin.Begin);

    [Benchmark]
    public Result Binarize() => pbo.Binarize(destination, new());

    [GlobalCleanup]
    public void Cleanup() => destination.Close();
}
