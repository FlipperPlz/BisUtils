namespace BisUtils.P3D.Models.Lod;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Core.Binarize;
using Core.Binarize.Implementation;
using BisUtils.Core.Binarize.Options;
using BisUtils.Core.Extensions;
using Core.IO;
using Core.Render.Vector;
using Data;
using Errors;
using Face;
using Point;
using Utils;
using Options;
using FResults;
using FResults.Extensions;
using FResults.Reasoning;

public interface IRVLod : IStrictBinaryObject<RVShapeOptions>
{
    public IRVResolution Resolution { get; set; }
    List<IVector3D> Normals { get; set; }
    List<IRVPoint> Points { get; set; }
    List<IRVFace> Faces { get; set; }
    List<IRVDataVertex> Vertices { get; set; }
    List<IRVNamedProperty> NamedProperties { get; }
    List<IRVNamedSelection> NamedSelections { get; }
    List<IRVAnimationPhase> AnimationPhases { get; }
    List<IRVSharpEdge> SharpEdges { get; }
    IRVPointAttrib<float> Mass { get; }
    IRVSelection? Selection { get; }
    IRVSelection? HiddenSelection { get; }
    IRVSelection? LockedSelection { get; }


    void AddAnimationPhase(IRVAnimationPhase phase);
}

public class RVLod : StrictBinaryObject<RVShapeOptions>, IRVLod
{
    protected const uint ValidVersion = 256;
    public IRVResolution Resolution { get; set; } = null!;
    public List<IRVPoint> Points { get; set; } = null!;
    public List<IVector3D> Normals { get; set; } = null!;
    public List<IRVFace> Faces { get; set; } = null!;
    public List<IRVDataVertex> Vertices { get; set; } = null!;
    public List<IRVNamedProperty> NamedProperties => new(RVConstants.MaxNamedProperties);
    public List<IRVNamedSelection> NamedSelections => new(RVConstants.MaxNamedSelections);
    public List<IRVAnimationPhase> AnimationPhases => new();
    public List<IRVSharpEdge> SharpEdges { get; private set; } = new();
    public IRVPointAttrib<float> Mass { get; set; } = null!;
    public IRVSelection? Selection { get; private set; }
    public IRVSelection? HiddenSelection { get; private set; }
    public IRVSelection? LockedSelection { get; private set; }


    public RVLod()
    {
    }

    public RVLod(BisBinaryReader reader, RVShapeOptions options) : base(reader, options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RVLod
    (
        IRVResolution resolution,
        List<IRVDataVertex> vertices,
        List<IRVPoint> points,
        List<IVector3D> normals,
        List<IRVFace> faces,
        IRVPointAttrib<float> mass
    )
    {
        Vertices = vertices;
        Resolution = resolution;
        Points = points;
        Normals = normals;
        Faces = faces;
        Mass = mass;
    }

    public void AddAnimationPhase(IRVAnimationPhase phase)
    {
        var i = AnimationPhases.FindIndex(p => p.Time > phase.Time);
        AnimationPhases.Insert(i != -1 ? i : AnimationPhases.Count, phase);
    }

    public override Result Binarize(BisBinaryWriter writer, RVShapeOptions options) => throw new NotImplementedException();

    public sealed override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
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

        Points = reader
            .ReadIndexedList<RVPoint, IBinarizationOptions>(options, pointCount)
            .Cast<IRVPoint>()
            .ToList();
        Normals = reader
            .ReadIndexedList<BinarizableVector3D, IBinarizationOptions>(options, normalCount)
            .Cast<IVector3D>()
            .ToList();
        Faces = reader
            .ReadStrictIndexedList<RVFace, RVShapeOptions>(options, facesCount)
            .Cast<IRVFace>()
            .ToList();
        return ReadLodBody(reader, options);
    }

    private void ReadMaterialIndex(string name, BinaryReader reader) =>
        NamedProperties.Add
        (
            new RVNamedProperty
            (
                name,
                reader.ReadInt32().ToString("x8", CultureInfo.CurrentCulture)
            )
        );

    private Result ReadLodBody(BisBinaryReader reader, RVShapeOptions options) => reader.ReadAscii(4, options) switch
    {
        "TAGG" => (LastResult ??= Result.Ok()).WithReasons(ReadTaggs(reader, options).Reasons),
        "SS3D" => (LastResult ??= Result.Ok()).WithReasons(ReadSS3D(reader, options).Reasons),
        _ => (LastResult ??= Result.Ok()).WithError(new LodReadError("Unknown setup magic."))
    };

    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private Result ReadSS3D(BisBinaryReader reader, RVShapeOptions options)
    {
        /*var nVertices = */reader.ReadInt32();
        /*var nFaces = */reader.ReadInt32();
        /*var nNormals =*/ reader.ReadInt32();
        /*var normSize =*/ reader.ReadInt32();
        //TODO(SS3D): Read
        throw new NotImplementedException();
    }

    private Result ReadTaggs(BisBinaryReader reader, RVShapeOptions options)
    {
        LastResult ??= Result.Ok();
        var shouldEndTaggs = false;
        int verticesSize = Points.Count * sizeof(byte), facesSize = Faces.Count * sizeof(bool);

        while (true)
        {
            var activated = reader.ReadBoolean();
            if (!reader.ReadAsciiZ(out var taggName, options))
            {
                return LastResult.WithError(new LodReadError("Tried to parse a tag without proper prefix."));
            }
            var tagLength = reader.ReadInt32();

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
                    var selection = new RVSelection(this);
                    selection.LoadSelection(reader, verticesSize, facesSize, 0);
                    Selection = selection;
                    break;
                }
                case "#Lock#":
                {
                    var selection = new RVSelection(this);
                    selection.LoadSelection(reader, verticesSize, facesSize, 0);
                    LockedSelection = selection;
                    break;
                }
                case "#Hide#":
                {
                    var selection = new RVSelection(this);
                    selection.LoadSelection(reader, verticesSize, facesSize, 0);
                    HiddenSelection = selection;
                    break;
                }
                case "#ShapeEdges#":
                {
                    var edgeCount = tagLength / 8;
                    SharpEdges = reader.ReadIndexedList<RVSharpEdge, RVShapeOptions>(options, edgeCount)
                        .Cast<IRVSharpEdge>()
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
                            var count = Points.Count;
                            if(count > 0)
                            {
                                Mass.Attributes = reader.ReadIndexedList(it => it.ReadSingle(), count).ToList();
                            }

                            break;
                        }
                    }

                    break;
                }
                case "#Animation#":
                {
                    AddAnimationPhase(new RVAnimationPhase(reader, options) { Parent = this });

                    break;
                }
                case "#Property#":
                {
                    NamedProperties.Add
                    (
                        new RVNamedProperty
                        (
                            reader.ReadChars(64).ToString()!.TrimEnd('\0', ' '),
                            reader.ReadChars(64).ToString()!.TrimEnd('\0', ' ')
                        )
                    );
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
                        var selection = new RVNamedSelection(this, taggName);
                        selection.LoadSelection(reader, verticesSize, facesSize, 0);
                        NamedSelections.Add(selection);
                    }

                    break;
                }
            }

            if (shouldEndTaggs)
            {
                break;
            }
        }

        Resolution = new RVResolution(reader, options);
        return LastResult;
    }

    public override Result Validate(RVShapeOptions options) => throw new NotImplementedException();
}
