namespace BisUtils.EnfPack.Options;

using System.Text;
using Core.Binarize.Options;
using Core.Binarize.Utils;

public class EsPackOptions : IBinarizationOptions
{
    public Encoding Charset { get; set; } = Encoding.UTF8;
    public Endianness ByteOrder { get; set; } = Endianness.Big;
    public bool IgnoreValidation { get; set; } = true;
}
