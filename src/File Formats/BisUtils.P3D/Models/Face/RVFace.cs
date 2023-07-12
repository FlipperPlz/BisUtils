namespace BisUtils.P3D.Models.Face;

using System.Text;
using Core.Binarize;using Core.Binarize.Implementation;
using Core.Binarize.Options;
using Core.Extensions;
using Core.IO;
using Errors;
using FResults;
using FResults.Extensions;
using Options;
using Utils;

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

    public RVFace()
    {

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
        #region Write Face Header

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

        #endregion
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        #region Read Face Header
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
                Vertices = reader.ReadIndexedList<RVDataVertex, IBinarizationOptions>(options);
                break;
            }
            case true:
            {
                switch (options.FaceVersion)
                {
                    case 1:
                    {
                        LastResult = Result.Ok();
                        Vertices = reader.ReadIndexedList<RVDataVertex, IBinarizationOptions>(options);
                        Flags = (RVFaceFlags)reader.ReadInt32();
                        LastResult.WithReasons(reader.ReadAsciiZ(out var texture, options).Reasons);
                        Texture = texture;
                        LastResult.WithReasons(reader.ReadAsciiZ(out var material, options).Reasons);
                        Material = material;
                        break;
                    }
                    default:
                    {//TODO(Validate): maybe not terminated
                        (LastResult = Result.Ok()).WithReasons(reader.ReadAsciiZ(out var texture, options).Reasons);
                        Texture = texture;
                        Vertices = reader.ReadIndexedList<RVDataVertex, IBinarizationOptions>(options);
                        break;
                    }
                }
                break;
            }
        }
        #endregion

        #region Read Face Body

        switch (reader.ReadAscii(4, options))
        {
            case "TAGG":
            {
                while (true)
                {
                    LexTagg(out var taggName, reader);
                    if (taggName is null || !taggName.StartsWith('#'))
                    {
                        return LastResult.WithError(new FaceReadError("Tried to parse a tag without proper prefix."));
                    }
                }
            }
            case "SS3D":
            {
                var nVertices = reader.ReadInt32();
                var nFaces = reader.ReadInt32();
                var nNormals = reader.ReadInt32();
                var normSize = reader.ReadInt32();
                //TODO(SS3D): Read
                throw new NotImplementedException();
            }
            default:
            {
                return LastResult.WithError(new FaceReadError("Unknown setup magic."));
            }
        }

        #endregion
    }

    private static Result LexTagg(out string? taggName, BisBinaryReader reader)
    {
        var taggBuilder = new StringBuilder("#");
        var currentChar = reader.ReadChar();
        if (currentChar != '#')
        {
            taggName = null;
            return Result.Fail(new FaceReadError("Expected '#'"));
        }

        do
        {
            taggBuilder.Append(currentChar = reader.ReadChar());
        } while (currentChar != '#');

        taggName = taggBuilder.ToString();

        return Result.Ok();
    }

    public override Result Validate(RVShapeOptions options) => LastResult = Result.FailIf(Vertices.Count() > 4, "Face has more than 4 vertices");
}


