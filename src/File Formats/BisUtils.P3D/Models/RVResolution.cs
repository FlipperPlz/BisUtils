namespace BisUtils.P3D.Models;

using System.Globalization;

public interface IRVResolution
{

}

public readonly struct RVResolution
{
    public float Value { get; }
    public RVLodType Type { get; }
    public bool KeepsNamedSelections { get; }
    public string Name { get; }
    public bool IsResolution { get; }
    public bool IsShadow { get; }
    public bool IsVisual { get; }

    public RVResolution(float value)
    {
        Value = value;
        Type = GetLodType(Value);
        Name = GetLodName(Value, Type);
        IsResolution = CanBeResolution(Value);
        IsShadow = CanBeShadow(Type);
        IsVisual = IsResolution || Value is ViewCargo or ViewPilot or ViewCommander;
        KeepsNamedSelections = Math.Abs(value - Buoyancy) < 0.00001 || ShouldKeepNamedSelections(Type);
    }

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

    public static bool CanBeResolution(float value) => value < ShadowVolume;

    public static bool CanBeShadow(RVLodType type) => type is RVLodType.ShadowVolume or RVLodType.ShadowVolumeViewGunner
        or RVLodType.ShadowVolumeViewPilot or RVLodType.ShadowVolumeViewGunner;
    public static bool WithinShadowRange(float value) => value is >= ShadowMin and <= ShadowMax;

    private static string GetLodName(float value, RVLodType? type = null)
    {
        type ??= GetLodType(value);
        if (type != RVLodType.ShadowVolume)
        {
            return (type == RVLodType.Resolution ? value.ToString("#.000", CultureInfo.CurrentCulture) : Enum.GetName(typeof(RVLodType), type)) ?? string.Empty;
        }

        return $"ShadowVolume{value - ShadowVolume}";
    }

    public static RVLodType GetLodType(float value) => value switch
    {
        Memory => RVLodType.Memory,
        LandContact => RVLodType.LandContact,
        Roadway => RVLodType.Roadway,
        Paths => RVLodType.Paths,
        HitPoints => RVLodType.HitPoints,
        ViewGeometry => RVLodType.ViewGeometry,
        FireGeometry => RVLodType.FireGeometry,
        ViewCargoGeometry => RVLodType.ViewCargoGeometry,
        ViewCargoFireGeometry => RVLodType.ViewCargoFireGeometry,
        ViewCommander => RVLodType.ViewCommander,
        ViewCommanderGeometry => RVLodType.ViewCommanderGeometry,
        ViewCommanderFireGeometry => RVLodType.ViewCommanderFireGeometry,
        ViewPilotGeometry => RVLodType.ViewPilotGeometry,
        ViewPilotFireGeometry => RVLodType.ViewPilotFireGeometry,
        ViewGunnerGeometry => RVLodType.ViewGunnerGeometry,
        ViewGunnerFireGeometry => RVLodType.ViewGunnerFireGeometry,
        SubParts => RVLodType.SubParts,
        ShadowVolumeViewCargo => RVLodType.ShadowVolumeViewCargo,
        ShadowVolumeViewPilot => RVLodType.ShadowVolumeViewPilot,
        ShadowVolumeViewGunner => RVLodType.ShadowVolumeViewGunner,
        Wreck => RVLodType.Wreck,
        ViewGunner => RVLodType.ViewGunner,
        ViewPilot => RVLodType.ViewPilot,
        ViewCargo => RVLodType.ViewCargo,
        Geometry => RVLodType.Geometry,
        PhysX => RVLodType.PhysX,
        _ => WithinShadowRange(value) ? RVLodType.ShadowVolume : RVLodType.Resolution
    };

    private const float SpecialLod = 1E+15f;
    public const float Buoyancy = 2E+13f;
    public const float PhysxOld = 3E+13f;
    public const float ShadowVolume = 10000f;
    public const float ShadowBuffer = 11000f;
    public const float Geometry = 1E+13f;
    public const float PhysX = 4E+13f;
    public const float Memory = 1E+15f;
    public const float LandContact = 2E+15f;
    public const float Roadway = 3E+15f;
    public const float Paths = 4E+15f;
    public const float HitPoints = 5E+15f;
    public const float ViewGeometry = 6E+15f;
    public const float FireGeometry = 7E+15f;
    public const float ViewCargoGeometry = 8E+15f;
    public const float ViewCargoFireGeometry = 9E15f;
    public const float ViewPilotGeometry = 1.3E+16f;
    public const float ViewPilotFireGeometry = 1.4E+16f;
    public const float ViewGunnerGeometry = 1.5E+16f;
    public const float ViewGunnerFireGeometry = 1.6E+16f;
    public const float SubParts = 1.7E+16f;
    public const float ShadowVolumeViewCargo = 1.8E+16f;
    public const float ShadowVolumeViewPilot = 1.9E+16f;
    public const float ShadowVolumeViewGunner = 2E+16f;
    public const float Wreck = 2.1E+16f;
    public const float ViewCommander = 1E+16f;
    public const float ViewCommanderGeometry = 1.1E+16f;
    public const float ViewCommanderFireGeometry = 1.2E+16f;
    public const float ViewGunner = 1000f;
    public const float ViewPilot = 1100f;
    public const float ViewCargo = 1200f;
    public const float ShadowMin = 10000f;
    public const float ShadowMax = 20000f;
}
