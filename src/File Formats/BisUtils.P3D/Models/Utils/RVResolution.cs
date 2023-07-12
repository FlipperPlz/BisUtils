// ReSharper disable UnusedMember.Local
namespace BisUtils.P3D.Models.Utils;

using System.Globalization;
using Lod;

public interface IRVResolution
{
    public string Name { get; }
    public RVLodTypes Type { get; }
    public float Value { get; }
    public bool KeepsNamedSelections { get; }
    public bool IsResolution { get; }
    public bool IsShadow { get; }
    public bool IsVisual { get; }

}

public readonly struct RVResolution : IRVResolution
{
    public float Value { get; }
    public RVLodTypes Type { get; }
    public bool KeepsNamedSelections { get; }
    public string Name { get; }
    public bool IsResolution { get; }
    public bool IsShadow { get; }
    public bool IsVisual { get; }

    public RVResolution(float value)
    {
        Value = value;
        Type = RVConstants.GetLodType(Value);
        Name = GetLodName(Value, Type);
        IsResolution = RVConstants.CanBeResolution(Value);
        IsShadow = RVConstants.CanBeShadow(Type);
        IsVisual = IsResolution || Type is RVLodTypes.ViewCargo or RVLodTypes.ViewPilot or RVLodTypes.ViewCommander;
        KeepsNamedSelections = RVConstants.CompareFloats(value, RVConstants.Buoyancy) || RVConstants.ShouldKeepNamedSelections(Type);
    }


    public static implicit operator float(RVResolution resolution) => resolution.Value;
    public static explicit operator RVResolution(float resolution) => new(resolution);

    private static string GetLodName(float value, RVLodTypes? type = null)
    {
        type ??= RVConstants.GetLodType(value);
        if (type != RVLodTypes.ShadowVolume)
        {
            return (type == RVLodTypes.Resolution ? value.ToString("#.000", CultureInfo.CurrentCulture) : Enum.GetName(typeof(RVLodTypes), type)) ?? string.Empty;
        }

        return $"ShadowVolume{value - RVConstants.ShadowBLod}";
    }
}
