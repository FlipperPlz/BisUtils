namespace BisUtils.RvBank.Benchmarks;

using BenchmarkDotNet.Attributes;
using BisUtils.Core.IO;
using Microsoft.Extensions.Logging;
using RvBank.Model;
using RvBank.Options;

public class RvBankCreationBenchmarks
{
    private readonly BisBinaryReader reader;
    private readonly ILogger logger;

    private readonly RvBankOptions flatReadOptions = new() { FlatRead = true };
    private readonly RvBankOptions options = new() { FlatRead = false };

    public RvBankCreationBenchmarks(ILogger logger)
    {
        this.logger = logger;
        var fs = File.OpenRead(@"C:\Steam\steamapps\common\DayZ\Addons\structures_data.pbo");
        reader = new BisBinaryReader(fs);
    }

    [IterationSetup(Target = nameof(DebinarizeFlatRead))]
    public void DebinarizeFlatReadSetup() => reader.BaseStream.Position = 0;

    [Benchmark(Baseline = true)]
    public RvBank DebinarizeFlatRead() => new("structures_data", reader, flatReadOptions, null, logger);

    [IterationSetup(Target = nameof(Debinarize))]
    public void DebinarizeSetup() => reader.BaseStream.Position = 0;

    [Benchmark]
    public RvBank Debinarize() => new("structures_data", reader, options, null, logger);

    [GlobalCleanup]
    public void Cleanup() => reader.Close();
}
