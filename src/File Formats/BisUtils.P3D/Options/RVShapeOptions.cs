namespace BisUtils.P3D.Options;

using System.Text;
using Core.Binarize.Options;
using Core.Binarize.Utils;
using Core.Options;

public class RVShapeOptions : IBinarizationOptions, IAsciizLimiterOptions
{
    public int MaxFacePolyCount { get; set; } = 4;
    public int MaxLodLevels { get; set; } = 32;
    public Encoding Charset { get; set; } = Encoding.UTF8;
    public Endianness ByteOrder { get; set; } = Endianness.Little;
    public bool IgnoreValidation { get; set; }
    public int AsciiLengthTimeout { get; set; }

    public int FaceVersion { get; set; }
    public int LodVersion { get; set; }
    public bool ExtendedFace { get; set; }
    public bool ExtendedPoint { get; set; }
    public int SelectionVerticesCount { get; set; }
    public int SelectionFaceCount { get; set; }
    public int SelectionNormalsCount { get; set; }

    public int LodMajorVersion => LodVersion >> 8;
    public int LodMinorVersion => LodVersion & 0xff;

}
