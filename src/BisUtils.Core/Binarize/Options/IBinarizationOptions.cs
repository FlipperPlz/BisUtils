using System.Text;
using BisUtils.Core.Binarize.Utils;
using BisUtils.Core.Options;

namespace BisUtils.Core.Binarize.Options;

public interface IBinarizationOptions : IBisOptions
{
    public Encoding Charset { get; set; }
    public Endianness ByteOrder { get; set; }
    public bool IgnoreValidation { get; set; }
}