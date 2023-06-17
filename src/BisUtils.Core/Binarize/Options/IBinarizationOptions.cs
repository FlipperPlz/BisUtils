namespace BisUtils.Core.Binarize.Options;

using System.Text;
using Core.Options;
using Utils;

public interface IBinarizationOptions : IBisOptions
{
    public Encoding Charset { get; set; }
    public Endianness ByteOrder { get; set; }
    public bool IgnoreValidation { get; set; }
}
