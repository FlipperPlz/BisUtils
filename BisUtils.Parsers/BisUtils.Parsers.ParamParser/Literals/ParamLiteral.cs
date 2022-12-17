using System.Text;
using Antlr4.Runtime;
using BisUtils.Core.Serialization;
using BisUtils.Parsers.ParamParser.Utils.Interfaces;

namespace BisUtils.Parsers.ParamParser.Literals;

public interface IParamLiteral : IBisSerializable<ParamSerializationOptions> {
}

public abstract class ParamLiteral<Literal, Ctx, Serialize> : IParamParsable<Ctx, Serialize>, IParamLiteral where Serialize : IBisContextSerializable<Ctx, Serialize> where Ctx : ParserRuleContext {
    public Literal Value;
    private IParamParsable<Ctx, Serialize> _paramParsableImplementation;

    public ParamLiteral() { }
    public ParamLiteral(Literal value) => Value = value;
    public ParamLiteral(Ctx value) => FromParserContext(value);
    public ParamLiteral(BinaryReader value) => ReadBinary(value);

    public abstract Serialize FromParserContext(Ctx context);
    public abstract void WriteString(StringBuilder builder, ParamSerializationOptions serializationOptions);
    public abstract IBisBinarizable ReadBinary(BinaryReader reader);
    public abstract void WriteBinary(BinaryWriter writer);
}