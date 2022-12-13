namespace BisUtils.Core.Serialization.Options; 

/// <summary>
/// Abstract base class for serialization options.
/// </summary>
/// <remarks>
/// This class represents the options for serializing" an object into a binary representation.
/// It extends the <see cref="BisCommonSerializationOptions"/> class and provides no additional functionality.
/// </remarks>
public abstract class BisSerializationOptions : BisCommonSerializationOptions {
    /// <summary>
    /// Defines the amount of indentation applied to the start of the current context being serialized.
    /// </summary>
    public int Indentation { get; set; } = 0;
}
