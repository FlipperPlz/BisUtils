namespace BisUtils.P3D.Extensions;

using Models.Data;
using Models.Lod;

public static class RvLodExtensions
{
    public static IRVShapeData? LocateLevel(this IRVLod lod, float resolution, out int foundLevelPosition)
    {
        var minDifference = Math.Abs(resolution) * 0.00001f;
        IRVShapeData? found = null;
        foundLevelPosition = -1;
        var levelPosition = -1;
        foreach (var level in lod.LodLevels)
        {
            levelPosition++;
            var difference = Math.Abs(resolution - level.Resolution.Value);
            if (!(minDifference >= difference))
            {
                continue;
            }

            minDifference = difference;
            found = level;
            foundLevelPosition = levelPosition;
        }

        return found;
    }
}
