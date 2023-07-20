namespace BisUtils.Core.Binarize.Options;

using System.Text;
using Core.Options;
using Utils;

/// <summary>
/// Defines options for binarization process.
/// </summary>
/// <remarks>
/// These options include charset encoding, byte order (endianness), and whether to ignore validation.
/// </remarks>
public interface IBinarizationOptions : IBisOptions
{
    /// <summary>
    /// Gets or sets the character encoding to use during the binarization process.
    /// </summary>
    public Encoding Charset { get; set; }

    /// <summary>
    /// Gets or sets the byte order (endianness) to use during the binarization process.
    /// </summary>
    public Endianness ByteOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore validation during the binarization process.
    /// </summary>
    public bool IgnoreValidation { get; set; }
}
