namespace BisUtils.RvShape.Models.Lod;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Binarize.Options;
using Core.Extensions;
using Core.IO;
using Core.Render.Vector;
using Errors;
using Data;
using Face;
using FResults;
using FResults.Extensions;
using FResults.Reasoning;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Options;
using Point;
using Utils;

public interface IRvLod : IStrictBinaryObject<RvShapeOptions>
{
    public IRvResolution Resolution { get; set; }
    List<IVector3D> Normals { get; set; }
    List<IRvPoint> Points { get; set; }
    List<IRvFace> Faces { get; set; }
    List<IRvNamedProperty> NamedProperties { get; }
    List<IRvNamedSelection> NamedSelections { get; }
    List<IRvAnimationPhase> AnimationPhases { get; }
    List<IRvSharpEdge> SharpEdges { get; }
    IRvPointAttrib<float>? Mass { get; }
    IRvSelection? Selection { get; }
    IRvSelection? HiddenSelection { get; }
    IRvSelection? LockedSelection { get; }


    void AddAnimationPhase(IRvAnimationPhase phase);
}

public class RvLod : StrictBinaryObject<RvShapeOptions>, IRvLod
{
    protected const uint ValidVersion = 256;
    public IRvResolution Resolution { get; set; } = null!;
    public List<IRvPoint> Points { get; set; } = null!;
    public List<IVector3D> Normals { get; set; } = null!;
    public List<IRvFace> Faces { get; set; } = null!;
    public List<IRvNamedProperty> NamedProperties { get; } = new(RvShapeConstants.MaxNamedProperties);
    public List<IRvNamedSelection> NamedSelections { get; } = new(RvShapeConstants.MaxNamedSelections);
    public List<IRvAnimationPhase> AnimationPhases => new();
    public List<IRvSharpEdge> SharpEdges { get; private set; } = new();
    public IRvPointAttrib<float>? Mass { get; set; }
    public IRvSelection? Selection { get; private set; }
    public IRvSelection? HiddenSelection { get; private set; }
    public IRvSelection? LockedSelection { get; private set; }

    public RvLod() : base(NullLogger.Instance)
    {
    }

    public RvLod(ILogger? logger) : base(logger)
    {
    }

    public RvLod(BisBinaryReader reader, RvShapeOptions options, ILogger? logger) : base(reader, options, logger)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RvLod
    (
        IRvResolution resolution,
        List<IRvPoint> points,
        List<IVector3D> normals,
        List<IRvFace> faces,
        IRvPointAttrib<float> mass,
        ILogger? logger
    ) : base(logger)
    {
        Resolution = resolution;
        Points = points;
        Normals = normals;
        Faces = faces;
        Mass = mass;
    }

    public void AddAnimationPhase(IRvAnimationPhase phase)
    {
        var i = AnimationPhases.FindIndex(p => p.Time > phase.Time);
        AnimationPhases.Insert(i != -1 ? i : AnimationPhases.Count, phase);
    }

    public override Result Binarize(BisBinaryWriter writer, RvShapeOptions options) => throw new NotImplementedException();

    public sealed override Result Debinarize(BisBinaryReader reader, RvShapeOptions options)
    {

        LastResult = Result.Ok();

        var magic = reader.ReadAscii(4, options);
        var headSize = reader.ReadInt32();
        options.FaceVersion = reader.ReadInt32();
        var pointCount = reader.ReadInt32();
        var normalCount = reader.ReadInt32();
        var facesCount = reader.ReadInt32();
        /*TODO: var flags = */reader.ReadInt32();

        options.ExtendedFace = false;

        switch (magic)
        {
            case "P3DM":
            {
                options.ExtendedFace = true;
                options.ExtendedPoint = true;
                if (options.FaceVersion != ValidVersion)
                {
                    //return Result.Warn()
                    throw new NotImplementedException();
                }

                options.FaceVersion = 1;

                reader.BaseStream.Seek(headSize - (24 + magic.Length), SeekOrigin.Current);
                break;
            }
            case "SP3X":
            {
                options.FaceVersion = 0;
                options.ExtendedFace = true;
                options.ExtendedPoint = true;
                reader.BaseStream.Seek(headSize - (24 + magic.Length), SeekOrigin.Current);
                break;
            }
            case "SP3D":
            {
                pointCount = headSize;
                normalCount = options.FaceVersion;
                facesCount = pointCount;

                // ReSharper disable once RedundantAssignment
                headSize = 12 + magic.Length;
                options.FaceVersion = 0;
                break;
            }
            default:
            {
                //return Result.Warn($"Bad file format {magic}");
                throw new NotImplementedException();
            }
        }
        //TODO(MAJOR): LOGGER USAGE IN READINDEXEDLIST ******PLS FIX
        Points = reader
            .ReadIndexedList<RvPoint, IBinarizationOptions>(options, pointCount)
            .Cast<IRvPoint>()
            .ToList();
        Normals = reader
            .ReadIndexedList<BinarizableVector3D, IBinarizationOptions>(options, normalCount)
            .Cast<IVector3D>()
            .ToList();

        Faces = reader
            .ReadStrictIndexedList<RvFace, RvShapeOptions>(options, facesCount, face =>
            {
                var i = 0;
                foreach (var vertex in face.Vertices)
                {
                    i++;
                    var absMapU = Math.Abs(vertex.MapU);
                    var absMapV = Math.Abs(vertex.MapV);
                    if (!(absMapU < 1e5) || !(absMapV < 1e5) || float.IsNaN(vertex.MapU) || float.IsNaN(vertex.MapV))
                    {
                        if (options.ReportInvalidFaceVertexUV)
                        {
                            LastResult.WithWarning("Invalid UV", typeof(RvDataVertex),
                                $"Face #{i}, Point #{vertex.Point}: Face points to ({face.Vertices[0].Point}, {face.Vertices[0].Point}, {face.Vertices[0].Point}) - Invalid UV (U:{vertex.MapU}, V:{vertex.MapV}) setting UV to zero.");
                        }

                        vertex.MapU = 0;
                        vertex.MapV = 0;
                    }

                    if (absMapU > options.UVLimit || absMapV > options.UVLimit)
                    {
                        LastResult.WithWarning("Oversized UV", typeof(RvDataVertex),
                            $"UV coordinate on point #{i} is too big (U:{vertex.MapU}, V:{vertex.MapV}, Current Max: {options.UVLimit}) - the UV compression may produce inaccurate results");
                    }
                }
            })
            .Cast<IRvFace>()
            .ToList();
        //Header has been read, prepare for tag reading.
        //CalculateUVMinMax(out var uvMinMax);

        LastResult = ReadLodBody(reader, options);
        Resolution = new RvResolution(reader, options, Logger);
        return LastResult;
    }


    private void AddNamedProperty(string name, string value) => AddNamedProperty(new RvNamedProperty(name, value));

    private void AddNamedProperty(IRvNamedProperty namedProperty) => NamedProperties.Add(namedProperty);

    private void ReadMaterialIndex(string name, BinaryReader reader) =>
        AddNamedProperty(name, reader.ReadInt32().ToString("x8", CultureInfo.CurrentCulture));

    //TODO: Update on Faces add or on Faces.Vertices add
    private void CalculateUVMinMax(out (float, float) minMaxUV)
    {
        (float, float) minUV, maxUV;
        minUV.Item1 = float.MaxValue;
        minUV.Item2 = float.MaxValue;
        maxUV.Item1 = float.MinValue;
        maxUV.Item2 = float.MinValue;
        foreach (var vertex in Faces.SelectMany(face => face.Vertices))
        {
            if (vertex.MapU > maxUV.Item1)
            {
                maxUV.Item1 = vertex.MapU;
            }

            if (vertex.MapV > maxUV.Item2)
            {
                maxUV.Item2 = vertex.MapV;
            }

            if (vertex.MapU > minUV.Item1)
            {
                minUV.Item1 = vertex.MapU;
            }

            if (vertex.MapV > minUV.Item2)
            {
                minUV.Item2 = vertex.MapV;
            }
        }
        maxUV.Item1 = Math.Max(maxUV.Item1, minUV.Item1);
        maxUV.Item2 = Math.Max(maxUV.Item2, minUV.Item2);
        (float, float) _minUV = new(), _maxUV = new();
        const float minIntervalSize = 1e-6f;
        if (maxUV.Item1 - minUV.Item1 < minIntervalSize)
        {
            _maxUV.Item1 = NextFloat(minUV.Item1 + minIntervalSize);
        }
        if (maxUV.Item2 - minUV.Item2 < minIntervalSize)
        {
            _maxUV.Item2 = NextFloat(minUV.Item2 + minIntervalSize);
        }

        minMaxUV = new(1 / (_maxUV.Item1 - _minUV.Item1), 1 / (_maxUV.Item2 - _minUV.Item2));
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct UFloatInt
    {
        [FieldOffset(0)]
        public float f;
        [FieldOffset(0)]
        public uint i;
    }

    internal static float NextFloat(float x)
    {
        UFloatInt h;
        h.f = x;
        h.i = 0;

        if ((h.i & 0x7F800000) == 0x7F800000)
        {
            if (h.i != 0xFF800000)
            {
                return x + x;
            }
        }

        else if (h.i == 0x80000000)
        {
            return 1.401298464e-45f;
        }

        if ((h.i & 0x80000000) == 0)
        {
            h.i++;
        } else
        {
            h.i--;
        }

        return h.f;
    }


    private Result ReadLodBody(BisBinaryReader reader, RvShapeOptions options) => reader.ReadAscii(4, options) switch
    {
        "TAGG" => (LastResult ??= Result.Ok()).WithReasons(ReadTaggs(reader, options).Reasons),
        "SS3D" => (LastResult ??= Result.Ok()).WithReasons(ReadSS3D(reader, options).Reasons),
        _ => (LastResult ??= Result.Ok()).WithError(new RvShapeLodReadError("Unknown setup magic."))
    };

    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private Result ReadSS3D(BisBinaryReader reader, RvShapeOptions options)
    {
        /*var nVertices = */ reader.ReadInt32();
        /*var nFaces = */ reader.ReadInt32();
        /*var nNormals =*/ reader.ReadInt32();
        /*var normSize =*/ reader.ReadInt32();
        //TODO(SS3D): Read
        throw new NotImplementedException();
    }

    private Result ReadTaggs(BisBinaryReader reader, RvShapeOptions options)
    {
        LastResult ??= Result.Ok();
        var shouldEndTaggs = false;

        while (true)
        {
            var activated = reader.ReadBoolean();
            if (!reader.ReadAsciiZ(out var taggName, options))
            {
                return LastResult.WithError(new RvShapeLodReadError("Tried to parse a tag without proper prefix."));
            }
            var tagLength = reader.ReadInt32();
            var endPosition = reader.BaseStream.Position + tagLength;

            if (!activated)
            {
                reader.BaseStream.Seek(tagLength, SeekOrigin.Current);
            }
            switch (taggName)
            {
                case "#EndOfFile#":
                {
                    shouldEndTaggs = true;
                    break;
                }
                case "#Selected#":
                {
                    var selection = new RvSelection(this);
                    selection.LoadSelection(reader, Points.Count, Faces.Count);
                    Selection = selection;
                    break;
                }
                case "#Lock#":
                {
                    var selection = new RvSelection(this);
                    selection.LoadSelection(reader, Points.Count, Faces.Count);
                    LockedSelection = selection;
                    break;
                }
                case "#Hide#":
                {
                    var selection = new RvSelection(this);
                    selection.LoadSelection(reader, Points.Count, Faces.Count);
                    HiddenSelection = selection;
                    break;
                }
                case "#SharpEdges#":
                {
                    var edgeCount = tagLength / 8;
                    SharpEdges = reader.ReadIndexedList<RvSharpEdge, RvShapeOptions>(options, edgeCount)
                        .Cast<IRvSharpEdge>()
                        .ToList();
                    SharpEdges.ForEach( it => it.OrganizeEdges());
                    SharpEdges.Sort
                    (
                        (x, y) =>
                        {
                            var ret = x.EdgeX - y.EdgeX;
                            if (ret != 0)
                            {
                                return ret;
                            }

                            return y.EdgeY - y.EdgeY;
                        }
                    );
                    break;
                }
                case "#UVSet#":
                {
                    //TODO: UVSet, skip for now
                    var stageId = reader.ReadInt32();
                    if (stageId > 1)
                    {
                        LastResult.WithWarning($"Unsupported UVSet Stage {stageId}");
                    }

                    foreach (var vertex in Faces.SelectMany(face => face.Vertices))
                    {
                        vertex.FaceUV = new RvUvMap(reader.ReadSingle(), reader.ReadSingle());
                    }
                    break;
                }
                case "#Mass#":
                {
                    switch (options.LodVersion)
                    {
                        case 0:
                        {
                            LastResult.WithReason(new Warning { Message = "Old mass no longer supported." });
                            reader.BaseStream.Seek(tagLength, SeekOrigin.Current);
                            break;
                        }
                        default:
                        {
                            var count = tagLength / 4;
                            if(count > 0)
                            {
                                Mass = new RvPointAttrib<float>(this,
                                    reader.ReadIndexedList(it => it.ReadSingle(), count).ToList());
                            }

                            break;
                        }
                    }

                    break;
                }
                case "#Animation#":
                {
                    AddAnimationPhase(new RvAnimationPhase(reader, options, Logger) { Parent = this });

                    break;
                }
                case "#Property#":
                {
                    AddNamedProperty(new string(reader.ReadChars(64)).TrimEnd('\0'), new string(reader.ReadChars(64)).TrimEnd('\0'));
                    break;
                }
                case "#MaterialIndex#":
                {
                    ReadMaterialIndex("__ambient", reader);
                    ReadMaterialIndex("__diffuse", reader);
                    ReadMaterialIndex("__specular", reader);
                    ReadMaterialIndex("__emissive", reader);
                    break;
                }
                default:
                {
                    if (taggName.StartsWith('#'))
                    {
                        LastResult.WithReason(new Warning { Message = $"Unknown or unsupported TAGG '{taggName}" });
                        reader.BaseStream.Seek(tagLength, SeekOrigin.Current);
                        break;
                    }

                    if (NamedSelections.Count < NamedSelections.Capacity)
                    {
                        var selection = new RvNamedSelection(this, taggName);
                        selection.LoadSelection(reader, Points.Count, Faces.Count);
                        NamedSelections.Add(selection);
                    }

                    break;
                }
            }

            if (reader.BaseStream.Position != endPosition)
            {
                LastResult.WithReason(new Warning { Message = $"Tagg {taggName} appears to have not been read fully." });

                reader.BaseStream.Seek(endPosition, SeekOrigin.Begin);
            }
            if (shouldEndTaggs)
            {
                break;
            }
        }

        return LastResult;
    }

    public override Result Validate(RvShapeOptions options) => throw new NotImplementedException();
}
