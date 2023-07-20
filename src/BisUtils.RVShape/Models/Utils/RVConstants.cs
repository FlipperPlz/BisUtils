namespace BisUtils.RVShape.Models.Utils;

using Lod;

public static class RVConstants
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

    public static RVLodType GetLodType(float f)
    {
        if (CompareFloats(f, MemoryLod))
        {
            return RVLodType.Memory;
        }

        if (CompareFloats(f, LandContactLod))
        {
            return RVLodType.LandContact;
        }

        if (CompareFloats(f, RoadWayLod))
        {
            return RVLodType.Roadway;
        }

        if (CompareFloats(f, PathsLod))
        {
            return RVLodType.Paths;
        }

        if (CompareFloats(f, PathsLod))
        {
            return RVLodType.Paths;
        }

        if (CompareFloats(f, HitPointsLod))
        {
            return RVLodType.HitPoints;
        }

        if (CompareFloats(f, ViewGeometryLod))
        {
            return RVLodType.ViewGeometry;
        }

        if (CompareFloats(f, FireGeometryLod))
        {
            return RVLodType.FireGeometry;
        }

        if (CompareFloats(f, ViewCargoFireGeometryLod))
        {
            return RVLodType.ViewCargoFireGeometry;
        }

        if (CompareFloats(f, ViewCommanderLod))
        {
            return RVLodType.ViewCommander;
        }

        if (CompareFloats(f, ViewCommanderGeometryLod))
        {
            return RVLodType.ViewCommanderGeometry;
        }

        if (CompareFloats(f, ViewCommanderFireGeometryLod))
        {
            return RVLodType.ViewCommanderFireGeometry;
        }

        if (CompareFloats(f, ViewPilotGeometryLod))
        {
            return RVLodType.ViewPilotGeometry;
        }

        if (CompareFloats(f, ViewPilotFireGeometryLod))
        {
            return RVLodType.ViewPilotFireGeometry;
        }

        if (CompareFloats(f, ViewGunnerGeometryLod))
        {
            return RVLodType.ViewGunnerGeometry;
        }

        if (CompareFloats(f, ViewGunnerFireGeometryLod))
        {
            return RVLodType.ViewGunnerFireGeometry;
        }

        if (CompareFloats(f, SubPartsLod))
        {
            return RVLodType.SubParts;
        }

        if (CompareFloats(f, ViewCargoShadowVolumeLod))
        {
            return RVLodType.ShadowVolumeViewCargo;
        }

        if (CompareFloats(f, ViewPilotShadowVolumeLod))
        {
            return RVLodType.ShadowVolumeViewPilot;
        }

        if (CompareFloats(f, ViewGunnerShadowVolumeLod))
        {
            return RVLodType.ShadowVolumeViewGunner;
        }

        if (CompareFloats(f, WreckLod))
        {
            return RVLodType.Wreck;
        }

        if (CompareFloats(f, ViewGunnerLod))
        {
            return RVLodType.ViewGunner;
        }

        if (CompareFloats(f, ViewPilotLod))
        {
            return RVLodType.ViewPilot;
        }

        if (CompareFloats(f, ViewCargoLod))
        {
            return RVLodType.ViewCargo;
        }

        if (CompareFloats(f, GeometryLod))
        {
            return RVLodType.Geometry;
        }

        if (CompareFloats(f, PhysXLod))
        {
            return RVLodType.PhysX;
        }

        if (WithinEditRange(f))
        {
            return RVLodType.Edit;
        }

        return WithinShadowRange(f) ? RVLodType.ShadowVolume : RVLodType.Undefined;
    }


    public static bool CanBeResolution(float value) => value < ShadowBLod;

    public static bool CanBeShadow(RVLodType type) => type is RVLodType.ShadowVolume or RVLodType.ShadowVolumeViewGunner
        or RVLodType.ShadowVolumeViewPilot or RVLodType.ShadowVolumeViewGunner;

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

    public static bool ShouldKeepNamedSelections(RVLodType type) =>
        type
            is RVLodType.Memory
            or RVLodType.FireGeometry
            or RVLodType.Geometry
            or RVLodType.ViewGeometry
            or RVLodType.ViewPilotGeometry
            or RVLodType.ViewGunnerGeometry
            or RVLodType.ViewCargoGeometry
            or RVLodType.Paths
            or RVLodType.HitPoints
            or RVLodType.PhysX;

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
