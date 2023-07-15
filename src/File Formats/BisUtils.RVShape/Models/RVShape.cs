namespace BisUtils.RVShape.Models;

using BisUtils.Core.Binarize;
using BisUtils.Core.Binarize.Implementation;
using BisUtils.Core.Extensions;
using BisUtils.Core.IO;
using BisUtils.Core.Render.Vector;
using BisUtils.P3D.Models.Utils;
using BisUtils.RVShape.Options;
using Errors;
using FResults;
using FResults.Extensions;
using Lod;
using Utils;

public interface IRVShape: IStrictBinaryObject<RVShapeOptions>
{
    string ModelName { get; set; }
    float Mass { get; }
    float InventoryMass { get; }
    IVector3D CenterOfMass { get; }
    List<IRVLod> LevelsOfDetail { get; set; }
    IRVLod? MemoryLod { get; }
    IRVLod? GeometryLod { get; }
    IRVLod? FireGeometryLod { get; }
    IRVLod? ViewGeometryLod { get; }
    IRVLod? PilotViewGeometryLod { get; }
    IRVLod? GunnerViewGeometryLod { get; }
    IRVLod? CargoViewGeometryLod { get; }
    IRVLod? LandContactLod { get; }
    IRVLod? RoadwayLod { get; }
    IRVLod? PathsLod { get; }
    IRVLod? HitPointsLod { get; }
    IRVLod? ShadowVolumeLod { get; }
    IRVLod? WreckLod { get; }
    IRVLod? CargoShadowVolumeLod { get; }
    IRVLod? PilotShadowVolumeLod { get; }
    IRVLod? GunnerShadowVolumeLod { get; }

    int ShadowBufferLodsCount { get; }
    int ShadowVolumeLodsCount { get; }
    int GraphicalLodsCount { get; }

}

public class RVShape : StrictBinaryObject<RVShapeOptions>, IRVShape
{
    public string ModelName { get; set; }
    public IRVLod? MemoryLod { get; private set; }
    public IRVLod? GeometryLod { get; private set; }
    public IRVLod? FireGeometryLod { get; private set; }
    public IRVLod? ViewGeometryLod { get; private set; }
    public IRVLod? PilotViewGeometryLod { get; private set; }
    public IRVLod? GunnerViewGeometryLod { get; private set; }
    public IRVLod? CargoViewGeometryLod { get; private set; }
    public IRVLod? LandContactLod { get; private set; }
    public IRVLod? RoadwayLod { get; private set; }
    public IRVLod? PathsLod { get; private set; }
    public IRVLod? HitPointsLod { get; private set; }
    public IRVLod? ShadowVolumeLod { get; private set; }
    public IRVLod? CargoShadowVolumeLod { get; private set; }
    public IRVLod? PilotShadowVolumeLod { get; private set; }
    public IRVLod? GunnerShadowVolumeLod { get; private set; }
    public IRVLod? WreckLod { get; private set; }
    public IRVLod? ShadowBufferLod { get; private set; }

    public int ShadowBufferLodsCount { get; private set; }
    public int ShadowVolumeLodsCount { get; private set; }
    public int GraphicalLodsCount { get; private set; }
    public float Mass { get; private set; }
    public float InventoryMass { get; private set; }
    public IVector3D CenterOfMass { get; set; } = null!;
    private List<IRVLod> lods = null!;

    public List<IRVLod> LevelsOfDetail
    {
        get => lods;
        set
        {
            lods = value;
            InitializeShape();
        }
    }

    public RVShape(string modelName, List<IRVLod> levelsOfDetail)
    {
        ModelName = modelName;
        LevelsOfDetail = levelsOfDetail;
    }

    public RVShape(string modelName, BisBinaryReader reader, RVShapeOptions options) : base(reader, options)
    {
        ModelName = modelName;
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, RVShapeOptions options) =>
        throw new NotImplementedException();

    public sealed override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
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
            default: return LastResult.WithError(new LodReadError("Unknown lod magic, expected ODOL, MLOD, or NLOD."));
        }
        LevelsOfDetail = reader
            .ReadStrictIndexedList<RVLod, RVShapeOptions>(options, levelCount)
            .Cast<IRVLod>()
            .ToList();
        if (LevelsOfDetail.Count == 1 && options.LodVersion < 0 && !isLod)
        {
            return LastResult.WithError(new LodReadError("Invalid P3D File!"));
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
                    case RVLodType.Geometry:
                    {
                        GeometryLod = lod;
                        break;
                    }
                    case RVLodType.Memory:
                    {
                        MemoryLod = lod;
                        break;
                    }
                    case RVLodType.LandContact:
                    {
                        LandContactLod = lod;
                        break;
                    }
                    case RVLodType.Roadway:
                    {
                        RoadwayLod = lod;
                        break;
                    }
                    case RVLodType.Paths:
                    {
                        PathsLod = lod;
                        break;
                    }
                    case RVLodType.HitPoints:
                    {
                        HitPointsLod = lod;
                        break;
                    }
                    case RVLodType.ShadowVolume:
                    {
                        ShadowVolumeLod ??= lod;

                        ShadowVolumeLodsCount++;
                        break;
                    }
                }

                if (RVConstants.WithinShadowBufferRange(resolution.Value))
                {
                    ShadowBufferLod ??= lod;

                    ShadowBufferLodsCount++;
                    break;
                }

                switch (resolution.Type)
                {
                    case RVLodType.ViewGeometry:
                    {
                        ViewGeometryLod = lod;
                        break;
                    }
                    case RVLodType.FireGeometry:
                    {
                        FireGeometryLod = lod;
                        break;
                    }
                    case RVLodType.ViewPilotGeometry:
                    {
                        PilotViewGeometryLod = lod;
                        break;
                    }
                    case RVLodType.ViewGunnerGeometry:
                    {
                        GunnerViewGeometryLod = lod;
                        break;
                    }
                    case RVLodType.ViewCargoGeometry:
                    {
                        CargoViewGeometryLod = lod;
                        break;
                    }
                    case RVLodType.Wreck:
                    {
                        WreckLod = lod;
                        break;
                    }
                    case RVLodType.ShadowVolumeViewCargo:
                    {
                        CargoShadowVolumeLod = lod;
                        break;
                    }
                    case RVLodType.ShadowVolumeViewPilot:
                    {
                        PilotShadowVolumeLod = lod;
                        break;
                    }
                    case RVLodType.ShadowVolumeViewGunner:
                    {
                        GunnerShadowVolumeLod = lod;
                        break;
                    }
                }
            }
        }
    }



    public override Result Validate(RVShapeOptions options) => throw new NotImplementedException();
}
