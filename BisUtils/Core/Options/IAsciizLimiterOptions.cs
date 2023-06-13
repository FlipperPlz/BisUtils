namespace BisUtils.Core.Options;

public interface IAsciizLimiterOptions : IBisOptions
{
    int AsciiLengthTimeout { get; set; }
}