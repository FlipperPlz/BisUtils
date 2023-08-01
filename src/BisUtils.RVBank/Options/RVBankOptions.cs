namespace BisUtils.RVBank.Options;

using System.Text;
using BisUtils.Core.Binarize.Options;
using Core.Binarize.Utils;
using BisUtils.Core.Options;
using Enumerations;

public class RVBankOptions : IBinarizationOptions, IAsciizLimiterOptions
{
    public TimeSpan DecompressionTimeout { get; set; } = TimeSpan.FromMinutes(1);
    public bool CompressionErrorsAreWarnings { get; set; } //= false;
    public bool RequireValidSignature { get; set; } = true;
    public bool RequireEmptyVersionMeta { get; set; } = true;
    public bool RequireFirstEntryIsVersion { get; set; } //= false;
    public bool WriteValidOffsets { get; set; } //= false;
    public bool AllowMultipleVersion { get; set; } //= false;
    public bool FlatRead { get; set; } = true; //= false;
    [FunctionallyAccurate] public bool AlwaysSeparateOnDummy { get; set; } = true;
    public bool IgnoreDuplicateFiles { get; set; } = true;
    public bool RegisterEmptyEntries { get; set; } = true;
    public bool AllowObfuscated { get; set; } //= false;
    public bool AllowEncrypted { get; set; } //= false;
    public bool AllowVersionMimeOnData { get; set; }
    public bool AllowUnnamedDataEntries { get; set; } = true;
    public bool IgnoreInvalidStreamSize { get; set; } // = false;
    public bool IgnoreEntryWhenLZSSOverflow { get; set; } // = false;
    [FunctionallyAccurate] public bool RequireVersionNotNamed { get; set; } = true;
    [FunctionallyAccurate] public bool RemoveBenignProperties { get; set; } = true;
    [FunctionallyAccurate] public bool RequireVersionMimeOnVersion { get; set; } = true;
    [FunctionallyAccurate] public bool RespectEntryOffsets { get; set; } = false;
    [FunctionallyAccurate] public int AsciiLengthTimeout { get; set; } = 1024;
    [FunctionallyAccurate] public Encoding Charset { get; set; } = Encoding.UTF8;
    [FunctionallyAccurate] public Endianness ByteOrder { get; set; } = Endianness.Little;
    public bool IgnoreValidation { get; set; } //= false;
}
