// ReSharper disable UnusedMember.Local
namespace BisUtils.P3D.Models.Utils;

using System.Globalization;
using Lod;

public interface IRVResolution
{
    public string Name { get; }
    public RVLodType Type { get; }
    public float Value { get; }
    public bool KeepsNamedSelections { get; }
    public bool IsResolution { get; }
    public bool IsShadow { get; }
    public bool IsVisual { get; }

}

public readonly struct RVResolution : IRVResolution
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
        Type = RVConstants.GetLodType(Value);
        Name = GetLodName(Value, Type);
        IsResolution = RVConstants.CanBeResolution(Value);
        IsShadow = RVConstants.CanBeShadow(Type);
        IsVisual = IsResolution || Type is RVLodType.ViewCargo or RVLodType.ViewPilot or RVLodType.ViewCommander;
        KeepsNamedSelections = RVConstants.CompareFloats(value, RVConstants.Buoyancy) || RVConstants.ShouldKeepNamedSelections(Type);
    }


    public static implicit operator float(RVResolution resolution) => resolution.Value;
    public static explicit operator RVResolution(float resolution) => new(resolution);

    private static string GetLodName(float value, RVLodType? type = null)
    {
        type ??= RVConstants.GetLodType(value);
        if (type != RVLodType.ShadowVolume)
        {
            return (type == RVLodType.Resolution ? value.ToString("#.000", CultureInfo.CurrentCulture) : Enum.GetName(typeof(RVLodType), type)) ?? string.Empty;
        }

        return $"ShadowVolume{value - RVConstants.ShadowBLod}";
    }
}
