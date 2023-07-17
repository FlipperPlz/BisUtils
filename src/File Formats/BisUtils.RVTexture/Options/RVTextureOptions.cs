namespace BisUtils.RVTexture.Options;

using System.Text;
using Core.Binarize.Options;
using Core.Binarize.Utils;

public class RVTextureOptions : IBinarizationOptions
{
    public Encoding Charset { get; set; } = Encoding.UTF8;
    public Endianness ByteOrder { get; set; }
    public bool IgnoreValidation { get; set; }
}
