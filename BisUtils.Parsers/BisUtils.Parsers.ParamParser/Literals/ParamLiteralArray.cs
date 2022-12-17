using System.Text;
using BisUtils.Core.Serialization;
using BisUtils.Parsers.ParamParser.Utils.Factories;
using BisUtils.Parsers.ParamParser.Utils.Interfaces;

namespace BisUtils.Parsers.ParamParser.Literals; 

public class ParamLiteralArray : ParamLiteral<List<IParamLiteral>, Generated.ParamLang.ParamParser.LiteralArrayContext, ParamLiteralArray> {
    public static readonly ParamLiteralArray EmptyArray = new ParamLiteralArray(new List<IParamLiteral>());
    
    public ParamLiteralArray() { }
    public ParamLiteralArray(List<IParamLiteral> value) : base(value) { }
    public ParamLiteralArray(Generated.ParamLang.ParamParser.LiteralArrayContext value) : base(value) { }
    public ParamLiteralArray(BinaryReader value) : base(value) { }

    public override ParamLiteralArray FromParserContext(Generated.ParamLang.ParamParser.LiteralArrayContext ctx) {
        Value = ctx.literalOrArray().Select(ParamLiteralFactory.Create).ToList();
        return this;
    }

    public override void WriteString(StringBuilder builder, ParamSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)))
            .Append('{')
            .Append(string.Join(',', Value.Select(v => v.ToString())))
            .Append('}');
    }

    public override IBisBinarizable ReadBinary(BinaryReader reader) {
        Value = new List<IParamLiteral>(reader.ReadCompactInteger());
        
        for (var i = 0; i < Value.Count; ++i) {
            switch (reader.ReadByte()) {
                case 0: { // String
                    Value.Add(reader.ReadBinarized<ParamLiteralString>());
                    break;
                }
                case 1: { // Float
                    Value.Add(reader.ReadBinarized<ParamLiteralFloat>());
                    break;
                }
                case 2: { // Integer
                    Value.Add(reader.ReadBinarized<ParamLiteralInteger>());
                    break;
                }
                case 3: { // Child Array
                    Value.Add(reader.ReadBinarized<ParamLiteralArray>());
                    break;
                }
                case 4: goto default;
                default: {
                    throw new Exception();
                }
            }
        }

        return this;
    }

    public override void WriteBinary(BinaryWriter writer) {
        var entriesList = Value.ToList();
        writer.WriteCompactInteger(entriesList.Count);

        foreach (var entry in entriesList) {
            switch (entry) {
                case ParamLiteralString @string: {
                    writer.Write((byte) 0);
                    @string.WriteBinary(writer);
                    continue;
                }
                case ParamLiteralFloat @float: {
                    writer.Write((byte) 1);
                    @float.WriteBinary(writer);
                    continue;
                }
                case ParamLiteralInteger @int: {
                    writer.Write((byte) 2);
                    @int.WriteBinary(writer);
                    continue;
                }
                case ParamLiteralArray array: {
                    writer.Write((byte) 3);
                    array.WriteBinary(writer);
                    continue;
                }
                default: throw new NotSupportedException();
            }
        }
    }
}