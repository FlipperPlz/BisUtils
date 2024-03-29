﻿namespace BisUtils.RVShape.Options;

using System.Text;
using BisUtils.Core.Binarize.Options;
using BisUtils.Core.Binarize.Utils;
using BisUtils.Core.Options;

public class RVShapeOptions : IBinarizationOptions, IAsciizLimiterOptions
{
    public int MaxFacePolyCount { get; set; } = 4;
    public int MaxLodLevels { get; set; } = 32;
    public Encoding Charset { get; set; } = Encoding.UTF8;
    public Endianness ByteOrder { get; set; } = Endianness.Little;
    public bool IgnoreValidation { get; set; }
    public int AsciiLengthTimeout { get; set; } = 9000;
    public bool ReportInvalidFaceVertexUV { get; set; } = true;
    public float UVLimit { get; set; } = 100.0f;


    public int FaceVersion { get; set; }
    public int LodVersion { get; set; }
    public bool ExtendedFace { get; set; }
    public bool ExtendedPoint { get; set; }
    public int LodMajorVersion => LodVersion >> 8;
    public int LodMinorVersion => LodVersion & 0xff;
}
