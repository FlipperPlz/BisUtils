// ReSharper disable UnusedMember.Local
namespace BisUtils.P3D.Models.Utils;

using System.Globalization;
using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Extensions;
using Core.IO;
using FResults;
using Lod;
using Options;

public interface IRVResolution : IBinaryObject<RVShapeOptions>
{
    public string Name { get; }
    public RVLodType Type { get; }
    public float Value { get; }
    public bool KeepsNamedSelections { get; }
    public bool IsResolution { get; }
    public bool IsShadow { get; }
    public bool IsVisual { get; }

}

public sealed class RVResolution : BinaryObject<RVShapeOptions>, IRVResolution
{
    private float value;
    public float Value
    {
        get => value;
        set
        {
            this.value = value;
            Type = RVConstants.GetLodType(this.value);
            Name = GetLodName(this.value, Type);
            IsResolution = RVConstants.CanBeResolution(this.value);
            IsShadow = RVConstants.CanBeShadow(Type);
            IsVisual = IsResolution || Type is RVLodType.ViewCargo or RVLodType.ViewPilot or RVLodType.ViewCommander;
            KeepsNamedSelections = RVConstants.CompareFloats(this.value, RVConstants.Buoyancy) || RVConstants.ShouldKeepNamedSelections(Type);
        }
    }

    public RVLodType Type { get; private set; }
    public bool KeepsNamedSelections { get; private set; }
    public string Name { get; private set; } = null!;
    public bool IsResolution { get; private set; }
    public bool IsShadow { get; private set; }
    public bool IsVisual { get; private set; }

    public RVResolution(float value) => Value = value;

    public RVResolution(BisBinaryReader reader, RVShapeOptions options) : base(reader, options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
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

    public override Result Binarize(BisBinaryWriter writer, RVShapeOptions options)
    {
        writer.Write(Value);
        return Result.Ok();
    }

    public override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        Value = reader.ReadSingle();
        return Result.Ok();
    }
}
