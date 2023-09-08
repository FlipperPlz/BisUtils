namespace BisUtils.RvShape.Models.Face;

using System.Globalization;
using Core.Binarize;
using Core.Binarize.Flagging;
using Core.Binarize.Implementation;
using Core.Extensions;
using Core.IO;
using FResults;
using FResults.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Options;
using Utils;

public interface IRvFace : IStrictBinaryObject<RvShapeOptions>, IBisOptionallyFlaggable<RvFaceFlag?>
{
    string Texture { get; set; }
    string? Material { get; set; }
    List<IRvDataVertex> Vertices { get; set; }
    bool IsOld { get; }
    bool IsExtended { get; }
}

public class RvFace : StrictBinaryObject<RvShapeOptions>, IRvFace
{
    public string Texture { get; set; } = null!;
    public string? Material { get; set; }
    public List<IRvDataVertex> Vertices { get; set; } = null!;
    public bool IsOld => Material is null;
    public bool IsExtended => this.IsFlaggable();
    public RvFaceFlag? Flags { get; set; }

    public RvFace(string texture, string? material, List<IRvDataVertex> vertices, RvFaceFlag[] flags, ILogger? logger) : base(logger)
    {
        Vertices = vertices;
        Texture = texture.ToLower(CultureInfo.CurrentCulture);
        Material = material?.ToLower(CultureInfo.CurrentCulture);
        Flags = BisFlagUtils.CreateFlagsFor<RvFaceFlag>(flags);
    }

    public RvFace(ILogger? logger) : base(logger)
    {

    }
    public RvFace() : base(NullLogger.Instance)
    {

    }

    public RvFace(BisBinaryReader reader, RvShapeOptions options, ILogger? logger) : base(reader, options, logger)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, RvShapeOptions options)
    {
        throw new NotImplementedException();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RvShapeOptions options)
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
                Vertices = reader.ReadIndexedList<RvDataVertex, RvShapeOptions>(options)
                    .Cast<IRvDataVertex>()
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
                        Vertices = reader.ReadIndexedList<RvDataVertex, RvShapeOptions>(options)
                            .Cast<IRvDataVertex>()
                            .ToList();
                        Flags = (RvFaceFlag) reader.ReadInt32();
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

    public override Result Validate(RvShapeOptions options) => LastResult = Result.Ok();
}


