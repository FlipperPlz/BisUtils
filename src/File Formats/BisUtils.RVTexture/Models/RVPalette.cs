namespace BisUtils.RVTexture.Models;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Extensions;
using Core.IO;
using Core.Render.Color;
using FResults;
using Options;

public interface IRVPalette : IBinaryObject<RVTextureOptions>
{
    public IColor MaxColor { get; set; }
    public IColor AverageColor { get; set; }
    public List<IColor> Colors { get; set; }

    public bool IsAlpha { get; set; }
    public bool IsTransparent { get; set; }
}

public class RVPalette : BinaryObject<RVTextureOptions>, IRVPalette
{
    public IColor MaxColor { get; set; } = null!;
    public IColor AverageColor { get; set; } = null!;
    public List<IColor> Colors { get; set; } = new();
    public bool IsAlpha { get; set; }
    public bool IsTransparent { get; set; }

    public RVPalette(BisBinaryReader reader, RVTextureOptions options) : base(reader, options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RVPalette(IColor maxColor, IColor averageColor, List<IColor> colors, bool isAlpha, bool isTransparent)
    {
        MaxColor = maxColor;
        AverageColor = averageColor;
        Colors = colors;
        IsAlpha = isAlpha;
        IsTransparent = isTransparent;
    }


    public override Result Binarize(BisBinaryWriter writer, RVTextureOptions options) => throw new NotImplementedException();

    public sealed override Result Debinarize(BisBinaryReader reader, RVTextureOptions options) => throw new NotImplementedException();
}
