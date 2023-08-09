namespace BisUtils.RvShape.Extensions;

using Models;
using Models.Lod;

public static class RvShapeExtensions
{
    public static IRvLod? LocateLevel(this IRvShape lod, float resolution, out int foundLevelPosition)
    {
        var minDifference = Math.Abs(resolution) * 0.00001f;
        IRvLod? found = null;
        foundLevelPosition = -1;
        var levelPosition = -1;
        foreach (var level in lod.LevelsOfDetail)
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
