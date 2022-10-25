using Antlr4.Runtime;

namespace BisUtils.Parsers.ParamParser.Interfaces; 

public interface IRapSerializable {
    public string ToString(int indentation = 0);
}