namespace BisUtils.RvShape.Models.Utils;

using Lod;

public static class RvShapeConstants
{
    public const float SpecialLod = 1e15f;
    public const int MaxNamedSelections = 0x200;
    public const int MaxNamedProperties = 0x80;

    public static readonly float MemoryLod = CalculateSpecialLod(1);
    public static readonly float LandContactLod = CalculateSpecialLod(2);
    public static readonly float RoadWayLod = CalculateSpecialLod(3);
    public static readonly float PathsLod = CalculateSpecialLod(4);
    public static readonly float HitPointsLod = CalculateSpecialLod(5);
    public static readonly float ViewGeometryLod = CalculateSpecialLod(6);
    public static readonly float FireGeometryLod = CalculateSpecialLod(7);
    public static readonly float ViewCargoGeometryLod = CalculateSpecialLod(8);
    public static readonly float ViewCargoFireGeometryLod = CalculateSpecialLod(9);

    public const float ViewCommanderLod = 1E+16f;
    public const float Buoyancy = 2E+13f;
    public const float ViewCommanderGeometryLod = 1.1E+16f;
    public const float ViewCommanderFireGeometryLod = 1.2E+16f;
    public const float ViewPilotGeometryLod = 1.3E+16f;
    public const float ViewPilotFireGeometryLod = 1.4E+16f;
    public const float ViewGunnerGeometryLod = 1.4999999E+16f;
    public const float ViewGunnerFireGeometryLod = 1.6E+16f;
    public const float SubPartsLod = 1.7E+16f;
    public const float ViewCargoShadowVolumeLod = 1.8E+16f;
    public const float ViewPilotShadowVolumeLod = 1.8E+16f;
    public const float ViewGunnerShadowVolumeLod = 2E+16f;
    public const float WreckLod = 2.1E+16f;
    public const float ViewGunnerLod = 1000.0f;
    public const float ViewPilotLod = 1100.0f;
    public const float ViewCargoLod = 1200.0f;
    public const float MinShadow = 10000.0f;
    public const float MaxShadow = 19999.9f;
    public const float MinShadowBuffer = 11000.0f;
    public const float MaxShadowBuffer = 11999.0f;
    public static readonly float ShadowBLod = MinShadow;
    public const float MinEditLod = 20000.0f;
    public static readonly float MaxEditLod = 29999.9f;
    public static readonly float EditBLod = MinEditLod;
    public const float GeometryLod = 1e13f;
    public const float PhysXLod = 1e13f;

    public static RvLodType GetLodType(float f)
    {
        if (CompareFloats(f, MemoryLod))
        {
            return RvLodType.Memory;
        }

        if (CompareFloats(f, LandContactLod))
        {
            return RvLodType.LandContact;
        }

        if (CompareFloats(f, RoadWayLod))
        {
            return RvLodType.Roadway;
        }

        if (CompareFloats(f, PathsLod))
        {
            return RvLodType.Paths;
        }

        if (CompareFloats(f, PathsLod))
        {
            return RvLodType.Paths;
        }

        if (CompareFloats(f, HitPointsLod))
        {
            return RvLodType.HitPoints;
        }

        if (CompareFloats(f, ViewGeometryLod))
        {
            return RvLodType.ViewGeometry;
        }

        if (CompareFloats(f, FireGeometryLod))
        {
            return RvLodType.FireGeometry;
        }

        if (CompareFloats(f, ViewCargoFireGeometryLod))
        {
            return RvLodType.ViewCargoFireGeometry;
        }

        if (CompareFloats(f, ViewCommanderLod))
        {
            return RvLodType.ViewCommander;
        }

        if (CompareFloats(f, ViewCommanderGeometryLod))
        {
            return RvLodType.ViewCommanderGeometry;
        }

        if (CompareFloats(f, ViewCommanderFireGeometryLod))
        {
            return RvLodType.ViewCommanderFireGeometry;
        }

        if (CompareFloats(f, ViewPilotGeometryLod))
        {
            return RvLodType.ViewPilotGeometry;
        }

        if (CompareFloats(f, ViewPilotFireGeometryLod))
        {
            return RvLodType.ViewPilotFireGeometry;
        }

        if (CompareFloats(f, ViewGunnerGeometryLod))
        {
            return RvLodType.ViewGunnerGeometry;
        }

        if (CompareFloats(f, ViewGunnerFireGeometryLod))
        {
            return RvLodType.ViewGunnerFireGeometry;
        }

        if (CompareFloats(f, SubPartsLod))
        {
            return RvLodType.SubParts;
        }

        if (CompareFloats(f, ViewCargoShadowVolumeLod))
        {
            return RvLodType.ShadowVolumeViewCargo;
        }

        if (CompareFloats(f, ViewPilotShadowVolumeLod))
        {
            return RvLodType.ShadowVolumeViewPilot;
        }

        if (CompareFloats(f, ViewGunnerShadowVolumeLod))
        {
            return RvLodType.ShadowVolumeViewGunner;
        }

        if (CompareFloats(f, WreckLod))
        {
            return RvLodType.Wreck;
        }

        if (CompareFloats(f, ViewGunnerLod))
        {
            return RvLodType.ViewGunner;
        }

        if (CompareFloats(f, ViewPilotLod))
        {
            return RvLodType.ViewPilot;
        }

        if (CompareFloats(f, ViewCargoLod))
        {
            return RvLodType.ViewCargo;
        }

        if (CompareFloats(f, GeometryLod))
        {
            return RvLodType.Geometry;
        }

        if (CompareFloats(f, PhysXLod))
        {
            return RvLodType.PhysX;
        }

        if (WithinEditRange(f))
        {
            return RvLodType.Edit;
        }

        return WithinShadowRange(f) ? RvLodType.ShadowVolume : RvLodType.Undefined;
    }


    public static bool CanBeResolution(float value) => value < ShadowBLod;

    public static bool CanBeShadow(RvLodType type) => type is RvLodType.ShadowVolume or RvLodType.ShadowVolumeViewGunner
        or RvLodType.ShadowVolumeViewPilot or RvLodType.ShadowVolumeViewGunner;

    public static bool WithinEditRange(float f) => f >= MinEditLod;

    public static bool WithinShadowRange(float f) => f is >= MinShadow and <= MaxShadow;


    public static bool WithinShadowBufferRange(float f) => f is >= MinShadowBuffer and <= MaxShadowBuffer;


    public static float CalculateSpecialLod(int x) =>
        SpecialLod * x;

    public static bool CompareFloats(float f1, float f2) => Math.Abs(f1 - f2) < 0.01f;

    public static float CalculateShadowLod(float x) =>
        x - MinShadow;

    public static float GetShadowLod(float x) =>
        x + MinShadow;

    public static float CalculateEditLod(float x) =>
        x - MinEditLod;

    public static float GetEditLod(float x) =>
        x + MinEditLod;

    public static bool IsMass(float f) =>
        (f > 1e12 && f < 1e14) || (f < -1e12 && f > -1e14);

    public static bool IsResolutionLod(float x) =>
        x < MinShadow || CompareFloats(x, ViewCommanderLod);

    public static bool ShouldKeepNamedSelections(RvLodType type) =>
        type
            is RvLodType.Memory
            or RvLodType.FireGeometry
            or RvLodType.Geometry
            or RvLodType.ViewGeometry
            or RvLodType.ViewPilotGeometry
            or RvLodType.ViewGunnerGeometry
            or RvLodType.ViewCargoGeometry
            or RvLodType.Paths
            or RvLodType.HitPoints
            or RvLodType.PhysX;

    public static bool IsGeometryLod(float x) =>
        CompareFloats(x, ViewGeometryLod) || CompareFloats(x, FireGeometryLod) ||
        CompareFloats(x, ViewCargoGeometryLod) || CompareFloats(x, ViewGunnerFireGeometryLod) ||
        CompareFloats(x, GeometryLod) || CompareFloats(x, ViewCargoFireGeometryLod) ||
        CompareFloats(x, ViewCommanderGeometryLod) ||CompareFloats(x, ViewCommanderFireGeometryLod) ||
        CompareFloats(x, ViewPilotGeometryLod) || CompareFloats(x, ViewPilotFireGeometryLod) ||
        CompareFloats(x, ViewGunnerGeometryLod);

    public static readonly float[] GeometryLodArray =
    {
        ViewGeometryLod, FireGeometryLod, ViewCargoGeometryLod, ViewCargoFireGeometryLod,
        ViewCommanderGeometryLod, ViewCommanderFireGeometryLod, ViewPilotGeometryLod,
        ViewPilotFireGeometryLod, ViewGunnerGeometryLod, ViewGunnerFireGeometryLod,
        GeometryLod
    };
}
