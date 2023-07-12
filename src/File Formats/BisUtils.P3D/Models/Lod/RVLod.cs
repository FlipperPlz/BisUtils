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
    public List<IRVShapeData> LodLevels { get; set; }
}

public class RVLod : BinaryObject<RVShapeOptions>, IRVLod
{
    public List<IRVShapeData> LodLevels { get; set; } = null!;

    public RVLod()
    {
    }

    public RVLod(BisBinaryReader reader, RVShapeOptions options) : base(reader, options)
    {

    }

    public RVLod(List<IRVShapeData> levels) => LodLevels = levels;

    public override Result Binarize(BisBinaryWriter writer, RVShapeOptions options) => throw new NotImplementedException();

    public override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        LastResult = Result.Ok();
        options.LodVersion = -1;
        var levelCount = options.MaxLodLevels;
        bool isLod;
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
        LodLevels = reader.ReadStrictIndexedList<RVShapeData, RVShapeOptions>(options, levelCount).Cast<IRVShapeData>().ToList();
        if (LodLevels.Count == 1 && options.LodVersion < 0 && !isLod)
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

        var firstLod = LodLevels.First();

        for (var x = 0; x < firstLod.PointCount(); x++)
        {
            if (!(firstLod.Mass[x] >= 0))
            {
                continue;
            }

            for( var i = 0; i < geometryLod.PointCount(); i++)
            {
                geometryLod.Mass[i] = 0;
            }

            for (var i = 0; i < firstLod.PointCount(); i++)
            {
                //double distanceSum = 0;
                var position0 = firstLod.Points[i];
                for (var j = 0; i < geometryLod.PointCount(); j++)
                {
                    var positionG = geometryLod.Points[j];
                    var distance = (positionG - position0).SquaredSize();
                    if (!(distance <= 1e-3))
                    {
                        //distanceSum += 1 / distance;
                        continue;
                    }

                    geometryLod.Mass[j] += firstLod.Mass[i];
                    goto EndLoop;

                }
                //var distanceSum2 = firstLod.Mass[i] ?? 0 / distanceSum;
                //float massSum = 0;
                //for (var j = 0; j < geometryLod.PointCount(); j++)
                //{
                //    var positionG = geometryLod.Points[j];
                //    var distance = (1 / (positionG - position0)).SquaredSize();
                //    massSum += (float)(distance * distanceSum);
                //}
                EndLoop:
                {

                    firstLod.Mass[i] = 0;
                }
            }
        }

        return LastResult;
    }


    public Result Validate(RVShapeOptions options) => throw new NotImplementedException();

}
