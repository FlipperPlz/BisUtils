using System.Text;
using BisUtils.Core.Serialization.Options;

namespace BisUtils.Core.Serialization; 

/// <summary>
/// Represents a serializable object that can be converted to and from a string representation.
/// </summary>
/// <typeparam name="DSO">The type of deserialization options to use.</typeparam>
/// <typeparam name="SO">The type of serialization options to use.</typeparam>
public interface IBisSerializable<in DSO, in SO> where DSO : BisDeserializationOptions where SO : BisSerializationOptions {
    /// <summary>
    /// Converts the specified string representation to an object.
    /// </summary>
    /// <param name="builder">The string representation to convert.</param>
    /// <param name="deserializationOptions">The deserialization options to use.</param>
    /// <returns>The deserialized object.</returns>
    public IBisBinarizable FromString(StringBuilder builder, DSO deserializationOptions);
    
    /// <summary>
    /// Converts the specified object to a string representation.
    /// </summary>
    /// <param name="builder">The string builder to use for storing the string representation.</param>
    /// <param name="serializationOptions">The serialization options to use.</param>
    public void Write(StringBuilder builder, SO serializationOptions);

    /// <summary>
    /// Converts the current object to a string representation using the specified options.
    /// </summary>
    /// <param name="options">The serialization options to use.</param>
    /// <returns>The string representation of the object.</returns>
    public virtual string ToString(SO options) {
        var builder = new StringBuilder();
        Write(builder, options);
        return builder.ToString();
    }
}

/// <summary>
/// Represents a serializable object that can be converted to and from a string representation.
/// </summary>
/// <typeparam name="CSO">The type of options to use.</typeparam>
public interface IBisSerializable<in CSO> where CSO : BisCommonSerializationOptions {
    /// <summary>
    /// Converts the specified string representation to an object.
    /// </summary>
    /// <param name="builder">The string representation to convert.</param>
    /// <param name="deserializationOptions">The deserialization options to use.</param>
    /// <returns>The deserialized object.</returns>
    public IBisBinarizable ReadString(StringBuilder builder, CSO deserializationOptions);
    
    /// <summary>
    /// Converts the specified object to a string representation.
    /// </summary>
    /// <param name="builder">The string builder to use for storing the string representation.</param>
    /// <param name="serializationOptions">The serialization options to use.</param>
    public void WriteString(StringBuilder builder, CSO serializationOptions);
    
    /// <summary>
    /// Converts the current object to a string representation using the specified options.
    /// </summary>
    /// <param name="options">The serialization options to use.</param>
    /// <returns>The string representation of the object.</returns>
    public virtual string ToString(CSO options) {
        var builder = new StringBuilder();
        WriteString(builder, options);
        return builder.ToString();
    }
}

/// <summary>
/// Represents a serializable object that can be converted to and from a string representation.
/// </summary>
public interface IBisSerializable {
    /// <summary>
    /// Converts the specified string representation to an object.
    /// </summary>
    /// <param name="builder">The string representation to convert.</param>
    /// <returns>The deserialized object.</returns>
    public IBisBinarizable ReadString(StringBuilder builder);
    
    /// <summary>
    /// Converts the specified object to a string representation.
    /// </summary>
    /// <param name="builder">The string builder to use for storing the string representation.</param>
    public void WriteString(StringBuilder builder);

    /// <summary>
    /// Converts the current object to a string representation using the specified options.
    /// </summary>
    /// <param name="options">The serialization options to use.</param>
    /// <returns>The string representation of the object.</returns>
    public virtual string ToString() {
        var builder = new StringBuilder();
        WriteString(builder);
        return builder.ToString();
    }
}