using Antlr4.Runtime;
using BisUtils.Core;
using BisUtils.Core.Serialization;

namespace BisUtils.Parsers.ParamParser.Interfaces;


public interface IRapSerializable : IBisSerializable<RapDeserializationOptions, RapSerializationOptions>, IBisBinarizable {
    
}