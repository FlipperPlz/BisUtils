using BisUtils.Core.Serialization.Options;

namespace BisUtils.Core.Serialization;

/// <summary>
/// An interface for objects that can be serialized and deserialized using binary format.
/// </summary>
/// <typeparam name="BO">The type of the binarization options to use for serialization and deserialization.</typeparam>
public interface IBisBinarizable<in BO> where BO : BisCommonBinarizationOptions {
    /// <summary>
    /// Reads the binary representation of an object and returns the debinarized object.
    /// </summary>
    /// <param name="reader">The binary reader to read from.</param>
    /// <param name="debinarizationOptions">The debinarization options to use.</param>
    /// <returns>The deserialized object.</returns>
    public IBisBinarizable<BO> ReadBinary(BinaryReader reader, BO debinarizationOptions);

    /// <summary>
    /// Writes the object to a binary writer using the specified binarization options.
    /// </summary>
    /// <param name="writer">The binary writer to write to.</param>
    /// <param name="binarizationOptions">The binarization options to use.</param>
    public void WriteBinary(BinaryWriter writer, BO binarizationOptions);
}


/// <summary>
/// An interface for objects that can be serialized and deserialized using binary format.
/// </summary>
/// <typeparam name="DBO">The type of the debinarization options to use for deserialization.</typeparam>
/// <typeparam name="BO">The type of the binarization options to use for serialization.</typeparam>
public interface IBisBinarizable<in DBO, in BO> where DBO : BisDebinarizationOptions where BO : BisBinarizationOptions {
    /// <summary>
    /// Reads the binary representation of an object and returns the debinarized object.
    /// </summary>
    /// <param name="reader">The binary reader to read from.</param>
    /// <param name="debinarizationOptions">The debinarization options to use.</param>
    /// <returns>The deserialized object.</returns>
    public IBisBinarizable<DBO, BO> ReadBinary(BinaryReader reader, DBO debinarizationOptions);
    /// <summary>
    /// Writes the object to a binary writer using the specified binarization options.
    /// </summary>
    /// <param name="writer">The binary writer to write to.</param>
    /// <param name="binarizationOptions">The binarization options to use.</param>
    public void WriteBinary(BinaryWriter writer, BO binarizationOptions);
}

public interface IBisBinarizable {
    public IBisBinarizable ReadBinary(BinaryReader reader);
    public void WriteBinary(BinaryWriter writer);
}

