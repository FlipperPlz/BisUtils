using System.Text;
using BisUtils.Core.Binarize.Options;
using BisUtils.Core.Binarize.Utils;
using BisUtils.Core.Options;

namespace BisUtils.Bank;

public class PboOptions : IBinarizationOptions, IAsciizLimiterOptions
{
    public Encoding Charset { get; set; } = Encoding.UTF8;
    public Endianness ByteOrder { get; set; } = Endianness.Little;
    public int AsciiLengthTimeout { get; set; } = 250;
    public TimeSpan DecompressionTimeout { get; set; } = TimeSpan.FromMinutes(1);
    public bool IgnoreValidation { get; set; } //= false;
    public bool CompressionErrorsAreWarnings { get; set; } //= false;
    public bool RequireValidSignature { get; set; } = true;
    public bool RequireEmptyVersionMeta { get; set; } = true;
    public bool RequireFirstEntryIsVersion { get; set; } //= false;
    public bool AllowMultipleVersion { get; set; } //= false;
    public bool AlwaysSeparateOnDummy { get; set; } = true;
    public bool AllowDuplicateFileNames { get; set; } //= false;
    public bool RegisterEmptyEntries { get; set; } = true;
    public bool AllowObfuscated { get; set; } //= false;
    public bool AllowEncrypted { get; set; } //= false;
    public bool AllowVersionMimeOnData { get; set; }
    public bool AllowUnnamedDataEntries { get; set; } = true;
    public bool IgnoreInvalidStreamSize { get; set; } = false;
    [FunctionallyAccurate] public bool RequireVersionNotNamed { get; set; } = true;
    [FunctionallyAccurate] public bool RemoveBenignProperties { get; set; } = true;
    [FunctionallyAccurate] public bool RequireVersionMimeOnVersion { get; set; } = true;
    [FunctionallyAccurate] public bool RespectEntryOffsets { get; set; } = false;
}
