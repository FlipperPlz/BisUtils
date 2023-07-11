namespace BisUtils.P3D.Models.Face;

using Utils;
using Core.Binarize;using Core.Binarize.Implementation;
using Core.Extensions;
using Core.IO;
using FResults;
using FResults.Extensions;
using Options;

public interface IRVFace : IStrictBinaryObject<RVShapeOptions>
{
    const int MaxDataCount = 4;
    string Texture { get; set; }
    string? Material { get; set; }
    IEnumerable<RVDataVertex> Vertices { get; set; }
    public RVFaceFlags? Flags { get; set; }

    public bool IsOld { get; }
    public bool IsExtended { get; }
}

public class RVFace : StrictBinaryObject<RVShapeOptions>, IRVFace
{
    public string Texture { get; set; } = null!;
    public string? Material { get; set; }
    public IEnumerable<RVDataVertex> Vertices { get; set; } = null!;
    public RVFaceFlags? Flags { get; set; }
    public bool IsOld => Material is null;
    public bool IsExtended => Flags is not null;

    public RVFace(string texture, string? material, IEnumerable<RVDataVertex> vertices, RVFaceFlags flags)
    {
        Texture = texture;
        Material = material;
        Vertices = vertices;
        Flags = flags;
    }

    public RVFace(BisBinaryReader reader, RVShapeOptions options) : base(reader, options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, RVShapeOptions options)
    {
        switch (options.ExtendedFace)
        {
            case false:
            {
                writer.WriteAsciiZ(Texture, options);
                writer.WriteAsciiZ(Material ?? "", options);
                Flags = null;
                return LastResult = Vertices.WriteBinarized(writer, options);
            }
            case true:
            {
                switch (options.FaceVersion)
                {
                    case 1:
                    {
                        LastResult = Vertices.WriteBinarized(writer, options);
                        writer.Write((int?)Flags ?? 0 );//TODO(Validate): ASSERT Flags When Extended
                        writer.WriteAsciiZ(Texture, options); //TODO(Validate): ASSERT V1 EXTENDED 9000 max
                        writer.WriteAsciiZ(Material ?? "", options); //TODO(Validate) ASSERT V1 EXTENDED 9000 max
                        return LastResult;
                    }
                    default:
                    {
                        writer.Write(Texture[..32]);//TODO(Validate): ASSERT NON-V1 EXTENDED 32 max
                        LastResult = Vertices.WriteBinarized(writer, options);
                        writer.Write((int?)Flags ?? 0 );
                        return LastResult;
                    }
                }
            }
        }
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        switch (options.ExtendedFace)
        {
            case false:
            {
                reader.ReadAsciiZ(out var texture, options);
                Texture = texture;
                reader.ReadAsciiZ(out var material, options);
                Material = material;
                Flags = null;
                LastResult = Result.Ok();
                var nVertices = reader.ReadInt32();
                var vertices = new List<RVDataVertex>(nVertices);
                for (var i = 0; i < nVertices; i++)
                {
                    var vertex = new RVDataVertex(reader, options);
                    vertices.Add(vertex);
                    if (vertex.LastResult?.Reasons is { } reasons)
                    {
                        LastResult.WithReasons(reasons);
                    }
                }

                Vertices = vertices;
                return LastResult;
            }
            case true:
            {
                switch (options.FaceVersion)
                {
                    case 1:
                    {
                        //TODO(REFACTOR): Create a method for reading a binarized list
                        LastResult = Result.Ok();
                        var nVertices = reader.ReadInt32();
                        var vertices = new List<RVDataVertex>(nVertices);
                        for (var i = 0; i < nVertices; i++)
                        {
                            var vertex = new RVDataVertex(reader, options);
                            vertices.Add(vertex);
                            if (vertex.LastResult?.Reasons is { } reasons)
                            {
                                LastResult.WithReasons(reasons);
                            }
                        }

                        Vertices = vertices;
                        Flags = (RVFaceFlags)reader.ReadInt32();
                        LastResult.WithReasons(reader.ReadAsciiZ(out var texture, options).Reasons);
                        Texture = texture;
                        LastResult.WithReasons(reader.ReadAsciiZ(out var material, options).Reasons);
                        Material = material;
                        return LastResult;
                    }
                    default:
                    {//TODO(Validate): maybe not terminated
                        (LastResult = Result.Ok()).WithReasons(reader.ReadAsciiZ(out var texture, options).Reasons);
                        Texture = texture;
                        var nVertices = reader.ReadInt32();
                        var vertices = new List<RVDataVertex>(nVertices);
                        for (var i = 0; i < nVertices; i++)
                        {
                            var vertex = new RVDataVertex(reader, options);
                            vertices.Add(vertex);
                            if (vertex.LastResult?.Reasons is { } reasons)
                            {
                                LastResult.WithReasons(reasons);
                            }
                        }
                        Vertices = vertices;
                        return LastResult;
                    }
                }
            }
        }
    }

    public override Result Validate(RVShapeOptions options) => LastResult = Result.FailIf(Vertices.Count() > 4, "Face has more than 4 vertices");
}


