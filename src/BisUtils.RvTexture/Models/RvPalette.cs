namespace BisUtils.RvTexture.Models;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Extensions;
using Core.IO;
using Core.Render.Color;
using FResults;
using Microsoft.Extensions.Logging;
using Options;

public interface IRvPalette : IBinaryObject<RvTextureOptions>
{
    public IColor MaxColor { get; set; }
    public IColor AverageColor { get; set; }
    public List<IColor> Colors { get; set; }

    public bool IsAlpha { get; set; }
    public bool IsTransparent { get; set; }
}

public class RvPalette : BinaryObject<RvTextureOptions>, IRvPalette
{
    public IColor MaxColor { get; set; } = null!;
    public IColor AverageColor { get; set; } = null!;
    public List<IColor> Colors { get; set; } = new();
    public bool IsAlpha { get; set; }
    public bool IsTransparent { get; set; }

    public RvPalette(BisBinaryReader reader, RvTextureOptions options, ILogger? logger) : base(reader, options, logger)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RvPalette(IColor maxColor, IColor averageColor, List<IColor> colors, bool isAlpha, bool isTransparent, ILogger? logger) : base(logger)
    {
        MaxColor = maxColor;
        AverageColor = averageColor;
        Colors = colors;
        IsAlpha = isAlpha;
        IsTransparent = isTransparent;
    }

    public override Result Binarize(BisBinaryWriter writer, RvTextureOptions options) => throw new NotImplementedException();

    public sealed override Result Debinarize(BisBinaryReader reader, RvTextureOptions options) => throw new NotImplementedException();
}
