namespace BisUtils.RvShape.Models;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Extensions;
using Core.IO;
using Core.Render.Vector;
using Errors;
using Lod;
using Utils;
using FResults;
using FResults.Extensions;
using Microsoft.Extensions.Logging;
using Options;

public interface IRvShape: IStrictBinaryObject<RvShapeOptions>
{
    string ModelName { get; set; }
    float Mass { get; }
    float InventoryMass { get; }
    IVector3D CenterOfMass { get; }
    List<IRvLod> LevelsOfDetail { get; set; }
    IRvLod? MemoryLod { get; }
    IRvLod? GeometryLod { get; }
    IRvLod? FireGeometryLod { get; }
    IRvLod? ViewGeometryLod { get; }
    IRvLod? PilotViewGeometryLod { get; }
    IRvLod? GunnerViewGeometryLod { get; }
    IRvLod? CargoViewGeometryLod { get; }
    IRvLod? LandContactLod { get; }
    IRvLod? RoadwayLod { get; }
    IRvLod? PathsLod { get; }
    IRvLod? HitPointsLod { get; }
    IRvLod? ShadowVolumeLod { get; }
    IRvLod? WreckLod { get; }
    IRvLod? CargoShadowVolumeLod { get; }
    IRvLod? PilotShadowVolumeLod { get; }
    IRvLod? GunnerShadowVolumeLod { get; }

    int ShadowBufferLodsCount { get; }
    int ShadowVolumeLodsCount { get; }
    int GraphicalLodsCount { get; }

}

public class RvShape : StrictBinaryObject<RvShapeOptions>, IRvShape
{
    public string ModelName { get; set; }
    public IRvLod? MemoryLod { get; private set; }
    public IRvLod? GeometryLod { get; private set; }
    public IRvLod? FireGeometryLod { get; private set; }
    public IRvLod? ViewGeometryLod { get; private set; }
    public IRvLod? PilotViewGeometryLod { get; private set; }
    public IRvLod? GunnerViewGeometryLod { get; private set; }
    public IRvLod? CargoViewGeometryLod { get; private set; }
    public IRvLod? LandContactLod { get; private set; }
    public IRvLod? RoadwayLod { get; private set; }
    public IRvLod? PathsLod { get; private set; }
    public IRvLod? HitPointsLod { get; private set; }
    public IRvLod? ShadowVolumeLod { get; private set; }
    public IRvLod? CargoShadowVolumeLod { get; private set; }
    public IRvLod? PilotShadowVolumeLod { get; private set; }
    public IRvLod? GunnerShadowVolumeLod { get; private set; }
    public IRvLod? WreckLod { get; private set; }
    public IRvLod? ShadowBufferLod { get; private set; }

    public int ShadowBufferLodsCount { get; private set; }
    public int ShadowVolumeLodsCount { get; private set; }
    public int GraphicalLodsCount { get; private set; }
    public float Mass { get; private set; }
    public float InventoryMass { get; private set; }
    public IVector3D CenterOfMass { get; set; } = null!;
    private List<IRvLod> lods = null!;

    public List<IRvLod> LevelsOfDetail
    {
        get => lods;
        set
        {
            lods = value;
            InitializeShape();
        }
    }

    public RvShape(string modelName, List<IRvLod> levelsOfDetail, ILogger? logger) : base(logger)
    {
        ModelName = modelName;
        LevelsOfDetail = levelsOfDetail;
    }

    public RvShape(string modelName, BisBinaryReader reader, RvShapeOptions options, ILogger? logger) : base(reader, options, logger)
    {
        ModelName = modelName;
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, RvShapeOptions options) =>
        throw new NotImplementedException();

    public sealed override Result Debinarize(BisBinaryReader reader, RvShapeOptions options)
    {
        LastResult = Result.Ok();
        options.LodVersion = -1;
        var levelCount = 1;
        bool isLod;
        Mass = 0;
        InventoryMass = 1e10f;
        CenterOfMass = new Vector3D(0, 0, 0);
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
                levelCount = reader.ReadInt32();
                break;
            }
            case "ODOL":
            {
                throw new NotImplementedException();
            }
            default: return LastResult.WithError(new RvShapeLodReadError("Unknown lod magic, expected ODOL, MLOD, or NLOD."));
        }
        LevelsOfDetail = reader
            .ReadStrictIndexedList<RvLod, RvShapeOptions>(options, levelCount)
            .Cast<IRvLod>()
            .ToList();
        if (LevelsOfDetail.Count == 1 && options.LodVersion < 0 && !isLod)
        {
            return LastResult.WithError(new RvShapeLodReadError("Invalid P3D File!"));
        }

        var major = options.LodMajorVersion;
        var minor = options.LodMinorVersion;
        return LastResult;
    }

    private void InitializeShape()
    {
        GraphicalLodsCount = lods.Count;
        ShadowBufferLodsCount = -1;
        ShadowVolumeLodsCount = -1;
        MemoryLod = null;
        ShadowVolumeLod = null;
        CargoShadowVolumeLod = null;
        GeometryLod = null;
        LandContactLod = null;
        RoadwayLod = null;
        PathsLod = null;
        HitPointsLod = null;
        ShadowBufferLod = null;
        ViewGeometryLod = null;
        FireGeometryLod = null;
        PilotViewGeometryLod = null;
        GunnerViewGeometryLod = null;
        CargoViewGeometryLod = null;
        WreckLod = null;
        PilotShadowVolumeLod = null;
        GunnerShadowVolumeLod = null;

        foreach (var lod in lods)
        {
            var resolution = lod.Resolution;
            if (resolution.Value > 900)
            {
                switch (resolution.Type)
                {
                    case RvLodType.Geometry:
                    {
                        GeometryLod = lod;
                        break;
                    }
                    case RvLodType.Memory:
                    {
                        MemoryLod = lod;
                        break;
                    }
                    case RvLodType.LandContact:
                    {
                        LandContactLod = lod;
                        break;
                    }
                    case RvLodType.Roadway:
                    {
                        RoadwayLod = lod;
                        break;
                    }
                    case RvLodType.Paths:
                    {
                        PathsLod = lod;
                        break;
                    }
                    case RvLodType.HitPoints:
                    {
                        HitPointsLod = lod;
                        break;
                    }
                    case RvLodType.ShadowVolume:
                    {
                        ShadowVolumeLod ??= lod;

                        ShadowVolumeLodsCount++;
                        break;
                    }
                }

                if (RvShapeConstants.WithinShadowBufferRange(resolution.Value))
                {
                    ShadowBufferLod ??= lod;

                    ShadowBufferLodsCount++;
                    break;
                }

                switch (resolution.Type)
                {
                    case RvLodType.ViewGeometry:
                    {
                        ViewGeometryLod = lod;
                        break;
                    }
                    case RvLodType.FireGeometry:
                    {
                        FireGeometryLod = lod;
                        break;
                    }
                    case RvLodType.ViewPilotGeometry:
                    {
                        PilotViewGeometryLod = lod;
                        break;
                    }
                    case RvLodType.ViewGunnerGeometry:
                    {
                        GunnerViewGeometryLod = lod;
                        break;
                    }
                    case RvLodType.ViewCargoGeometry:
                    {
                        CargoViewGeometryLod = lod;
                        break;
                    }
                    case RvLodType.Wreck:
                    {
                        WreckLod = lod;
                        break;
                    }
                    case RvLodType.ShadowVolumeViewCargo:
                    {
                        CargoShadowVolumeLod = lod;
                        break;
                    }
                    case RvLodType.ShadowVolumeViewPilot:
                    {
                        PilotShadowVolumeLod = lod;
                        break;
                    }
                    case RvLodType.ShadowVolumeViewGunner:
                    {
                        GunnerShadowVolumeLod = lod;
                        break;
                    }
                }
            }
        }
    }



    public override Result Validate(RvShapeOptions options) => throw new NotImplementedException();
}
