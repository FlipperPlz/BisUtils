namespace BisUtils.P3D.Options;

using System.Text;
using Core.Binarize.Options;
using Core.Binarize.Utils;
using Core.Options;

public class RVShapeOptions : IBinarizationOptions, IAsciizLimiterOptions
{
    public Encoding Charset { get; set; } = Encoding.UTF8;
    public Endianness ByteOrder { get; set; } = Endianness.Little;
    public bool IgnoreValidation { get; set; }
    public int AsciiLengthTimeout { get; set; }
}
