namespace BisUtils.P3D.Models.Face;

using System.Globalization;
using System.Text;
using Core.Binarize;
using Core.Binarize.Implementation;
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
    string Texture { get; set; }
    string? Material { get; set; }
    int? Flags { get; set; }
    List<IRVDataVertex> Vertices { get; set; }
    bool IsOld { get; }
    bool IsExtended { get; }

    bool HasFlag(RVFaceFlag flag);
}

public class RVFace : StrictBinaryObject<RVShapeOptions>, IRVFace
{
    public string Texture { get; set; } = null!;
    public string? Material { get; set; }
    public List<IRVDataVertex> Vertices { get; set; } = null!;
    public int? Flags { get; set; }
    public bool IsOld => Material is null;
    public bool IsExtended => Flags is not null;
    public bool HasFlag(RVFaceFlag flag) => (Flags & (int)flag) == (int)flag;

    public RVFace(string texture, string? material, List<IRVDataVertex> vertices, int flag)
    {
        Vertices = vertices;
        Texture = texture.ToLower(CultureInfo.CurrentCulture);
        Material = material?.ToLower(CultureInfo.CurrentCulture);
        Flags = flag;
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
        throw new NotImplementedException();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        #region Read Face Header
        switch (options.ExtendedFace)
        {
            case false:
            {
                reader.ReadAsciiZ(out var texture, options);
                Texture = texture.ToLower(CultureInfo.CurrentCulture);
                reader.ReadAsciiZ(out var material, options);
                Material = material.ToLower(CultureInfo.CurrentCulture);
                Vertices = reader.ReadIndexedList<RVDataVertex, RVShapeOptions>(options)
                    .Cast<IRVDataVertex>()
                    .ToList();
                Flags = null;
                LastResult = Result.Ok();
                break;
            }
            case true:
            {
                switch (options.FaceVersion)
                {
                    case 1:
                    {
                        LastResult = Result.Ok();
                        Vertices = reader.ReadIndexedList<RVDataVertex, RVShapeOptions>(options)
                            .Cast<IRVDataVertex>()
                            .ToList();
                        Flags = reader.ReadInt32();
                        LastResult.WithReasons(reader.ReadAsciiZ(out var texture, options).Reasons);
                        Texture = texture.ToLower(CultureInfo.CurrentCulture);
                        LastResult.WithReasons(reader.ReadAsciiZ(out var material, options).Reasons);
                        Material = material.ToLower(CultureInfo.CurrentCulture);
                        break;
                    }
                    default:
                    {//TODO(Validate): maybe not terminated
                        (LastResult = Result.Ok()).WithReasons(reader.ReadAsciiZ(out var texture, options).Reasons);
                        Texture = texture.ToLower(CultureInfo.CurrentCulture);
                        break;
                    }
                }
                break;
            }
        }
        #endregion

        return LastResult;
    }

    public override Result Validate(RVShapeOptions options) => LastResult = Result.Ok();
}


