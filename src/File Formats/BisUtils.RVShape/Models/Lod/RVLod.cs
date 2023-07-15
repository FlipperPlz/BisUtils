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
    List<IRVNamedProperty> NamedProperties { get; }
    List<IRVNamedSelection> NamedSelections { get; }
    List<IRVAnimationPhase> AnimationPhases { get; }
    List<IRVSharpEdge> SharpEdges { get; }
    IRVPointAttrib<float>? Mass { get; }
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
    public List<IRVNamedProperty> NamedProperties => new(RVConstants.MaxNamedProperties);
    public List<IRVNamedSelection> NamedSelections => new(RVConstants.MaxNamedSelections);
    public List<IRVAnimationPhase> AnimationPhases => new();
    public List<IRVSharpEdge> SharpEdges { get; private set; } = new();
    public IRVPointAttrib<float>? Mass { get; set; }
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
        List<IRVPoint> points,
        List<IVector3D> normals,
        List<IRVFace> faces,
        IRVPointAttrib<float> mass
    )
    {
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
            .ReadStrictIndexedList<RVFace, RVShapeOptions>(options, facesCount, face =>
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
                            LastResult.WithWarning("Invalid UV", typeof(RVDataVertex),
                                $"Face #{i}, Point #{vertex.Point}: Face points to ({face.Vertices[0].Point}, {face.Vertices[0].Point}, {face.Vertices[0].Point}) - Invalid UV (U:{vertex.MapU}, V:{vertex.MapV}) setting UV to zero.");
                        }

                        vertex.MapU = 0;
                        vertex.MapV = 0;
                    }

                    if (absMapU > options.UVLimit || absMapV > options.UVLimit)
                    {
                        LastResult.WithWarning("Oversized UV", typeof(RVDataVertex),
                            $"UV coordinate on point #{i} is too big (U:{vertex.MapU}, V:{vertex.MapV}, Current Max: {options.UVLimit}) - the UV compression may produce inaccurate results");
                    }
                }
            })
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
                    selection.LoadSelection(reader, Points.Count, Faces.Count);
                    Selection = selection;
                    break;
                }
                case "#Lock#":
                {
                    var selection = new RVSelection(this);
                    selection.LoadSelection(reader, Points.Count, Faces.Count);
                    LockedSelection = selection;
                    break;
                }
                case "#Hide#":
                {
                    var selection = new RVSelection(this);
                    selection.LoadSelection(reader, Points.Count, Faces.Count);
                    HiddenSelection = selection;
                    break;
                }
                case "#SharpEdges#":
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
                case "#UVSet#":
                {
                    //TODO: UVSet, skip for now
                    reader.BaseStream.Seek(tagLength, SeekOrigin.Current);
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
                                Mass = new RVPointAttrib<float>(this,
                                    reader.ReadIndexedList(it => it.ReadSingle(), count).ToList());
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
                        selection.LoadSelection(reader, Points.Count, Faces.Count);
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
