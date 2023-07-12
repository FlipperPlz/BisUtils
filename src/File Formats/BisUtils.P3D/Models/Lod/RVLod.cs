namespace BisUtils.P3D.Models.Lod;

using Core.Binarize.Implementation;
using Core.IO;
using Data;
using Errors;
using Extensions;
using FResults;
using FResults.Extensions;
using Options;
using Utils;

public interface IRVLod
{
    public IEnumerable<IRVShapeData> LodLevels { get; set; }

}

public class RVLod : BinaryObject<RVShapeOptions>, IRVLod
{
    public IEnumerable<IRVShapeData> LodLevels { get; set; } = null!;

    public RVLod()
    {
    }

    public RVLod(BisBinaryReader reader, RVShapeOptions options) : base(reader, options)
    {

    }

    public RVLod(IEnumerable<IRVShapeData> levels) => LodLevels = levels;

    public override Result Binarize(BisBinaryWriter writer, RVShapeOptions options) => throw new NotImplementedException();

    public override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        LastResult = Result.Ok();
        options.LodVersion = -1;
        var levelCount = options.MaxLodLevels;
        var isLod = false;
        switch (reader.ReadAscii(4, options))
        {
            case "NLOD":
            {
                isLod = true;
                levelCount = reader.ReadInt32();
                options.LodVersion = 0;
                break;
            }
            case "MLOD":
            {
                isLod = true;
                options.LodVersion = reader.ReadInt32();
                if (options.LodVersion == 9999)
                {
                    options.LodVersion = 1;
                }
                break;
            }
            case "ODOL":
            {
                throw new NotImplementedException();
            }
            default: return LastResult.WithError(new LodReadError("Unknown lod magic, expected ODOL, MLOD, or NLOD."));
        }
        LodLevels = reader.ReadStrictIndexedList<RVShapeData, RVShapeOptions>(options, levelCount);
        if (LodLevels.Count() == 1 && options.LodVersion < 0 && !isLod)
        {
            return LastResult.WithError(new LodReadError("Invalid P3D File!"));
        }

        if (
            this.LocateLevel(RVConstants.GeometryLod, out var levelPosition) is not { } geometryLod ||
            levelPosition <= 0 ||
            options.LodVersion != 0
            )
        {
            return LastResult;
        }
        //TODO: Deal with mass

        return LastResult;
    }


    public Result Validate(RVShapeOptions options) => throw new NotImplementedException();

}
