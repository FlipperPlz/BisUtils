using System.Collections;
using System.Text;
using Antlr4.Runtime;
using BisUtils.Core;
using BisUtils.Generated.ParamLang;
using BisUtils.Parsers.ParamParser.Factories;
using BisUtils.Parsers.ParamParser.Interfaces;

namespace BisUtils.Parsers.ParamParser.Literals; 

public class RapArray : IRapArrayEntry, IEnumerable<IRapArrayEntry>, IRapDeserializable<Generated.ParamLang.ParamParser.LiteralArrayContext> {
    public static readonly RapArray EmptyArray = new RapArray(new List<IRapArrayEntry>());
    public List<IRapArrayEntry> Entries { get; set; }

    public RapArray(List<IRapArrayEntry> entries) => Entries = entries;
    
    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.LiteralArrayContext ctx) {
        Entries = ctx.literalOrArray().Select(RapLiteralFactory.Create).ToList();
        return this;
    }
    
    public IEnumerator<IRapArrayEntry> GetEnumerator() => Entries.GetEnumerator(); 
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public RapArray() {}

    public static RapArray FromContext(Generated.ParamLang.ParamParser.LiteralArrayContext ctx) =>
        (RapArray) new RapArray().ReadParseTree(ctx);

    public IBisBinarizable FromString(StringBuilder builder, RapDeserializationOptions deserializationOptions) {
        var lexer = new ParamLexer(CharStreams.fromString(builder.ToString()));
        var tokens = new CommonTokenStream(lexer);
        var parser = new Generated.ParamLang.ParamParser(tokens);

        ReadParseTree(parser.literalArray());
        if (parser.NumberOfSyntaxErrors != 0) throw new Exception();
        
        return this;
    }

    public void Write(StringBuilder builder, RapSerializationOptions serializationOptions) =>
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)))
        .Append('{')
        .Append(string.Join(',', Entries.Select(v => v.ToString())))
        .Append('}');

    public IBisBinarizable ReadBinary(BinaryReader reader) {
        Entries = new List<IRapArrayEntry>(reader.ReadCompactInteger());
        
        for (var i = 0; i < Entries.Capacity; ++i) {
            switch (reader.ReadByte()) {
                case 0: { // String
                    Entries.Add(reader.ReadBinarized<RapString>());
                    break;
                }
                case 1: { // Float
                    Entries.Add(reader.ReadBinarized<RapFloat>());
                    break;
                }
                case 2: { // Integer
                    Entries.Add(reader.ReadBinarized<RapInteger>());
                    break;
                }
                case 3: { // Child Array
                    Entries.Add(reader.ReadBinarized<RapArray>());
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

    public void WriteBinary(BinaryWriter writer) {
        var entriesList = Entries.ToList();
        writer.WriteCompactInteger(entriesList.Count);

        foreach (var entry in entriesList) {
            switch (entry) {
                case RapString @string: {
                    writer.Write((byte) 0);
                    @string.WriteBinary(writer);
                    continue;
                }
                case RapFloat @float: {
                    writer.Write((byte) 1);
                    @float.WriteBinary(writer);
                    continue;
                }
                case RapInteger @int: {
                    writer.Write((byte) 2);
                    @int.WriteBinary(writer);
                    continue;
                }
                case RapArray array: {
                    writer.Write((byte) 3);
                    array.WriteBinary(writer);
                    continue;
                }
                default: throw new NotSupportedException();
            }
        }
    }
}