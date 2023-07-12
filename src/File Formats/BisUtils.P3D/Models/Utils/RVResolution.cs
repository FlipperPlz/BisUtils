// ReSharper disable UnusedMember.Local
namespace BisUtils.P3D.Models.Utils;

using System.Globalization;
using Lod;

public interface IRVResolution
{
    public string Name { get; }
    public RVLodFlags Flags { get; }
    public float Value { get; }
    public bool KeepsNamedSelections { get; }
    public bool IsResolution { get; }
    public bool IsShadow { get; }
    public bool IsVisual { get; }
}

public readonly struct RVResolution : IRVResolution
{
    public float Value { get; }
    public RVLodFlags Flags { get; }
    public bool KeepsNamedSelections { get; }
    public string Name { get; }
    public bool IsResolution { get; }
    public bool IsShadow { get; }
    public bool IsVisual { get; }

    public RVResolution(float value)
    {
        Value = value;
        Flags = GetLodType(Value);
        Name = GetLodName(Value, Flags);
        IsResolution = CanBeResolution(Value);
        IsShadow = CanBeShadow(Flags);
        IsVisual = IsResolution || Value is ViewCargo or ViewPilot or ViewCommander;
        KeepsNamedSelections = Math.Abs(value - Buoyancy) < 0.00001 || ShouldKeepNamedSelections(Flags);
    }

    public static bool ShouldKeepNamedSelections(RVLodFlags flags) =>
        flags
            is RVLodFlags.Memory
            or RVLodFlags.FireGeometry
            or RVLodFlags.Geometry
            or RVLodFlags.ViewGeometry
            or RVLodFlags.ViewPilotGeometry
            or RVLodFlags.ViewGunnerGeometry
            or RVLodFlags.ViewCargoGeometry
            or RVLodFlags.Paths
            or RVLodFlags.HitPoints
            or RVLodFlags.PhysX;

    public static bool CanBeResolution(float value) => value < ShadowVolume;

    public static bool CanBeShadow(RVLodFlags flags) => flags is RVLodFlags.ShadowVolume or RVLodFlags.ShadowVolumeViewGunner
        or RVLodFlags.ShadowVolumeViewPilot or RVLodFlags.ShadowVolumeViewGunner;
    public static bool WithinShadowRange(float value) => value is >= ShadowMin and <= ShadowMax;

    private static string GetLodName(float value, RVLodFlags? type = null)
    {
        type ??= GetLodType(value);
        if (type != RVLodFlags.ShadowVolume)
        {
            return (type == RVLodFlags.Resolution ? value.ToString("#.000", CultureInfo.CurrentCulture) : Enum.GetName(typeof(RVLodFlags), type)) ?? string.Empty;
        }

        return $"ShadowVolume{value - ShadowVolume}";
    }


    public static implicit operator float(RVResolution resolution) => resolution.Value;
    public static explicit operator RVResolution(float resolution) => new(resolution);

    public static RVLodFlags GetLodType(float value) => value switch
    {
        Memory => RVLodFlags.Memory,
        LandContact => RVLodFlags.LandContact,
        Roadway => RVLodFlags.Roadway,
        Paths => RVLodFlags.Paths,
        HitPoints => RVLodFlags.HitPoints,
        ViewGeometry => RVLodFlags.ViewGeometry,
        FireGeometry => RVLodFlags.FireGeometry,
        ViewCargoGeometry => RVLodFlags.ViewCargoGeometry,
        ViewCargoFireGeometry => RVLodFlags.ViewCargoFireGeometry,
        ViewCommander => RVLodFlags.ViewCommander,
        ViewCommanderGeometry => RVLodFlags.ViewCommanderGeometry,
        ViewCommanderFireGeometry => RVLodFlags.ViewCommanderFireGeometry,
        ViewPilotGeometry => RVLodFlags.ViewPilotGeometry,
        ViewPilotFireGeometry => RVLodFlags.ViewPilotFireGeometry,
        ViewGunnerGeometry => RVLodFlags.ViewGunnerGeometry,
        ViewGunnerFireGeometry => RVLodFlags.ViewGunnerFireGeometry,
        SubParts => RVLodFlags.SubParts,
        ShadowVolumeViewCargo => RVLodFlags.ShadowVolumeViewCargo,
        ShadowVolumeViewPilot => RVLodFlags.ShadowVolumeViewPilot,
        ShadowVolumeViewGunner => RVLodFlags.ShadowVolumeViewGunner,
        Wreck => RVLodFlags.Wreck,
        ViewGunner => RVLodFlags.ViewGunner,
        ViewPilot => RVLodFlags.ViewPilot,
        ViewCargo => RVLodFlags.ViewCargo,
        Geometry => RVLodFlags.Geometry,
        PhysX => RVLodFlags.PhysX,
        _ => WithinShadowRange(value) ? RVLodFlags.ShadowVolume : RVLodFlags.Resolution
    };

    private const float SpecialLod = 1E+15f;
    public const float Buoyancy = 2E+13f;
    public const float PhysxOld = 3E+13f;
    public const float ShadowBuffer = 11000f;

    public const float ShadowVolume = 10000f;
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
