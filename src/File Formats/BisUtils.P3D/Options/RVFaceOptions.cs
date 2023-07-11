namespace BisUtils.P3D.Options;

using System.Text;
using Core.Binarize.Options;
using Core.Binarize.Utils;
using Core.Options;

public class RVFaceOptions : IBinarizationOptions, IAsciizLimiterOptions
{
    public Encoding Charset { get; set; }
    public Endianness ByteOrder { get; set; }
    public bool IgnoreValidation { get; set; }
    public int AsciiLengthTimeout { get; set; }

    public bool ExtendedFace { get; set; }
    public int FaceVersion { get; set; }
}
