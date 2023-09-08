// ReSharper disable UnusedMember.Local
namespace BisUtils.RvShape.Models.Utils;

using System.Globalization;
using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Extensions;
using Core.IO;
using Lod;
using Options;
using FResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

public interface IRvResolution : IBinaryObject<RvShapeOptions>
{
    public string Name { get; }
    public RvLodType Type { get; }
    public float Value { get; }
    public bool KeepsNamedSelections { get; }
    public bool IsResolution { get; }
    public bool IsShadow { get; }
    public bool IsVisual { get; }

}

public sealed class RvResolution : BinaryObject<RvShapeOptions>, IRvResolution
{
    private float value;
    public float Value
    {
        get => value;
        set
        {
            this.value = value;
            Type = RvShapeConstants.GetLodType(this.value);
            Name = GetLodName(this.value, Type);
            IsResolution = RvShapeConstants.CanBeResolution(this.value) || Type == RvLodType.ViewCommander;
            IsShadow = RvShapeConstants.CanBeShadow(Type);
            IsVisual = IsResolution || Type is RvLodType.ViewCargo or RvLodType.ViewPilot or RvLodType.ViewCommander;
            KeepsNamedSelections = RvShapeConstants.CompareFloats(this.value, RvShapeConstants.Buoyancy) || RvShapeConstants.ShouldKeepNamedSelections(Type);
        }
    }

    public RvLodType Type { get; private set; }
    public bool KeepsNamedSelections { get; private set; }
    public string Name { get; private set; } = null!;
    public bool IsResolution { get; private set; }
    public bool IsShadow { get; private set; }
    public bool IsVisual { get; private set; }

    public RvResolution(float value, ILogger? logger) : base(logger) => Value = value;

    public RvResolution(BisBinaryReader reader, RvShapeOptions options, ILogger? logger) : base(reader, options, logger)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public static implicit operator float(RvResolution resolution) => resolution.Value;
    public static explicit operator RvResolution(float resolution) => new(resolution, NullLogger.Instance);

    private static string GetLodName(float value, RvLodType? type = null)
    {
        type ??= RvShapeConstants.GetLodType(value);
        if (type != RvLodType.ShadowVolume)
        {
            return (type == RvLodType.Resolution ? value.ToString("#.000", CultureInfo.CurrentCulture) : Enum.GetName(typeof(RvLodType), type)) ?? string.Empty;
        }

        return $"ShadowVolume{value - RvShapeConstants.ShadowBLod}";
    }

    public override Result Binarize(BisBinaryWriter writer, RvShapeOptions options)
    {
        writer.Write(Value);
        return Result.Ok();
    }

    public override Result Debinarize(BisBinaryReader reader, RvShapeOptions options)
    {
        Value = reader.ReadSingle();
        return Result.Ok();
    }
}
