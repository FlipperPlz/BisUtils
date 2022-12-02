using System.Text;
using Antlr4.Runtime;
using BisUtils.Core;

namespace BisUtils.Parsers.ParamParser.Interfaces; 

public interface IRapDeserializable<in T> : IRapSerializable where T : ParserRuleContext  {
    public IRapSerializable ReadParseTree(T ctx);
}