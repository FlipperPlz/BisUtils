using Antlr4.Runtime;
using BisUtils.Core.Serialization.Options;

namespace BisUtils.Core.Serialization; 

/// <summary>
/// An interface for serializing objects from a ParserRuleContext.
/// </summary>
/// <typeparam name="Ctx">The type of the ParserRuleContext.</typeparam>
/// <typeparam name="Serializable">The type of the serializable object.</typeparam>
public interface IBisContextSerializable<in Ctx, out Serializable> : IBisSerializable where Ctx : ParserRuleContext where Serializable : IBisContextSerializable<Ctx, Serializable> {
    /// <summary>
    /// Deserializes an object from a ParserRuleContext.
    /// </summary>
    /// <param name="context">The ParserRuleContext to deserialize from.</param>
    /// <returns>The deserialized object.</returns>
    public Serializable FromParserContext(Ctx context);
    
}

/// <summary>
/// An interface for serializing objects from a ParserRuleContext with deserialization options.
/// </summary>
/// <typeparam name="Ctx">The type of the ParserRuleContext.</typeparam>
/// <typeparam name="Serializable">The type of the serializable object.</typeparam>
/// <typeparam name="SerializationSettings">The type of the serialization options.</typeparam>
public interface IBisContextSerializable<in Ctx, out Serializable, in SerializationSettings> : 
    IBisContextSerializable<Ctx, Serializable>, 
    IBisSerializable<SerializationSettings> where Ctx : ParserRuleContext where Serializable : IBisContextSerializable<Ctx, Serializable> where SerializationSettings : BisCommonSerializationOptions {
}
