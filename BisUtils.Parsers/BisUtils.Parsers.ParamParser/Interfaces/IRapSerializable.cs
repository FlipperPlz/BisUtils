using Antlr4.Runtime;
using BisUtils.Core;

namespace BisUtils.Parsers.ParamParser.Interfaces;


public interface IRapSerializable : IBisSerializable<RapDeserializationOptions, RapSerializationOptions>, IBisBinarizable {
    
}