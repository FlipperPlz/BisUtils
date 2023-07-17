namespace BisUtils.Core.Render.Color.Options;

using System.Text;
using Binarize.Utils;

public class BisColorOptions : IBisColorOptions
{
    public Encoding Charset { get; set; } = Encoding.UTF8;
    public Endianness ByteOrder { get; set; }
    public bool IgnoreValidation { get; set; }
    public bool UsePackedColor { get; set; }
}
