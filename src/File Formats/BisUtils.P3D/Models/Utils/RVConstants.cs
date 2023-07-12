namespace BisUtils.P3D.Models.Utils;

using Lod;

public static class RVConstants
{
    public const float SpecialLod = 1e15f;
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
    public const float MinShadowLod = 10000.0f;
    public const float MaxShadowLod = 19999.9f;
    public static readonly float ShadowBLod = MinShadowLod;
    public const float MinEditLod = 20000.0f;
    public static readonly float MaxEditLod = 29999.9f;
    public static readonly float EditBLod = MinEditLod;
    public const float GeometryLod = 1e13f;
    public const float PhysXLod = 1e13f;

    public static RVLodTypes GetLodType(float f)
    {
        if (CompareFloats(f, MemoryLod))
        {
            return RVLodTypes.Memory;
        }

        if (CompareFloats(f, LandContactLod))
        {
            return RVLodTypes.LandContact;
        }

        if (CompareFloats(f, RoadWayLod))
        {
            return RVLodTypes.Roadway;
        }

        if (CompareFloats(f, PathsLod))
        {
            return RVLodTypes.Paths;
        }

        if (CompareFloats(f, PathsLod))
        {
            return RVLodTypes.Paths;
        }

        if (CompareFloats(f, HitPointsLod))
        {
            return RVLodTypes.HitPoints;
        }

        if (CompareFloats(f, ViewGeometryLod))
        {
            return RVLodTypes.ViewGeometry;
        }

        if (CompareFloats(f, FireGeometryLod))
        {
            return RVLodTypes.FireGeometry;
        }

        if (CompareFloats(f, ViewCargoFireGeometryLod))
        {
            return RVLodTypes.ViewCargoFireGeometry;
        }

        if (CompareFloats(f, ViewCommanderLod))
        {
            return RVLodTypes.ViewCommander;
        }

        if (CompareFloats(f, ViewCommanderGeometryLod))
        {
            return RVLodTypes.ViewCommanderGeometry;
        }

        if (CompareFloats(f, ViewCommanderFireGeometryLod))
        {
            return RVLodTypes.ViewCommanderFireGeometry;
        }

        if (CompareFloats(f, ViewPilotGeometryLod))
        {
            return RVLodTypes.ViewPilotGeometry;
        }

        if (CompareFloats(f, ViewPilotFireGeometryLod))
        {
            return RVLodTypes.ViewPilotFireGeometry;
        }

        if (CompareFloats(f, ViewGunnerGeometryLod))
        {
            return RVLodTypes.ViewGunnerGeometry;
        }

        if (CompareFloats(f, ViewGunnerFireGeometryLod))
        {
            return RVLodTypes.ViewGunnerFireGeometry;
        }

        if (CompareFloats(f, SubPartsLod))
        {
            return RVLodTypes.SubParts;
        }

        if (CompareFloats(f, ViewCargoShadowVolumeLod))
        {
            return RVLodTypes.ShadowVolumeViewCargo;
        }

        if (CompareFloats(f, ViewPilotShadowVolumeLod))
        {
            return RVLodTypes.ShadowVolumeViewPilot;
        }

        if (CompareFloats(f, ViewGunnerShadowVolumeLod))
        {
            return RVLodTypes.ShadowVolumeViewGunner;
        }

        if (CompareFloats(f, WreckLod))
        {
            return RVLodTypes.Wreck;
        }

        if (CompareFloats(f, ViewGunnerLod))
        {
            return RVLodTypes.ViewGunner;
        }

        if (CompareFloats(f, ViewPilotLod))
        {
            return RVLodTypes.ViewPilot;
        }

        if (CompareFloats(f, ViewCargoLod))
        {
            return RVLodTypes.ViewCargo;
        }

        if (CompareFloats(f, GeometryLod))
        {
            return RVLodTypes.Geometry;
        }

        if (CompareFloats(f, PhysXLod))
        {
            return RVLodTypes.PhysX;
        }

        return WithinShadowRange(f) ? RVLodTypes.ShadowVolume : RVLodTypes.Undefined;
    }


    public static bool CanBeResolution(float value) => value < ShadowBLod;

    public static bool CanBeShadow(RVLodTypes types) => types is RVLodTypes.ShadowVolume or RVLodTypes.ShadowVolumeViewGunner
        or RVLodTypes.ShadowVolumeViewPilot or RVLodTypes.ShadowVolumeViewGunner;

    public static bool WithinShadowRange(float f) => f is >= MinShadowLod and <= MaxShadowLod;


    public static float CalculateSpecialLod(int x) =>
        SpecialLod * x;

    public static bool CompareFloats(float f1, float f2) => Math.Abs(f1 - f2) < 0.01f;

    public static float CalculateShadowLod(float x) =>
        x - MinShadowLod;

    public static float GetShadowLod(float x) =>
        x + MinShadowLod;

    public static float CalculateEditLod(float x) =>
        x - MinEditLod;

    public static float GetEditLod(float x) =>
        x + MinEditLod;

    public static bool IsMass(float f) =>
        (f > 1e12 && f < 1e14) || (f < -1e12 && f > -1e14);

    public static bool IsResolutionLod(float x) =>
        x < MinShadowLod || CompareFloats(x, ViewCommanderLod);

    public static bool ShouldKeepNamedSelections(RVLodTypes types) =>
        types
            is RVLodTypes.Memory
            or RVLodTypes.FireGeometry
            or RVLodTypes.Geometry
            or RVLodTypes.ViewGeometry
            or RVLodTypes.ViewPilotGeometry
            or RVLodTypes.ViewGunnerGeometry
            or RVLodTypes.ViewCargoGeometry
            or RVLodTypes.Paths
            or RVLodTypes.HitPoints
            or RVLodTypes.PhysX;

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
