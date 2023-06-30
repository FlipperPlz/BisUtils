namespace BisUtils.Core.Binarize.Utils;

/// <summary>
/// Specifies the byte order of binary data being processed.
/// This can be used when reading from or writing to binary streams.
/// </summary>
public enum Endianness
{
    /// <summary>
    /// Indicates that the most significant byte (MSB) is at the lowest address,
    /// also known as Big-Endian.
    /// </summary>
    Big,
    /// <summary>
    /// Indicates that the least significant byte (LSB) is at the lowest address,
    /// also known as Little-Endian.
    /// </summary>
    Little
}
