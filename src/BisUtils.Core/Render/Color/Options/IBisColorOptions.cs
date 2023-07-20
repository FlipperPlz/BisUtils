namespace BisUtils.Core.Render.Color.Options;

using Binarize.Options;

public interface IBisColorOptions : IBinarizationOptions
{
    public bool UsePackedColor { get; set; }
}
