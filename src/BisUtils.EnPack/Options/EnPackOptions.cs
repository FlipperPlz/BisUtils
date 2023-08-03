namespace BisUtils.EnPack.Options;

using System.Text;
using BisUtils.Core.Binarize.Options;
using BisUtils.Core.Binarize.Utils;

public class EnPackOptions : IBinarizationOptions
{
    public Encoding Charset { get; set; } = Encoding.UTF8;
    public Endianness ByteOrder { get; set; } = Endianness.Big;
    public bool IgnoreValidation { get; set; } = true;
}
