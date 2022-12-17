using System.Text;
using Antlr4.Runtime;
using BisUtils.Core.Serialization;
using BisUtils.Core.Serialization.Options;
using BisUtils.Generated.ParamLang;

namespace BisUtils.Parsers.ParamParser.Utils.Interfaces;

public class ParamSerializationOptions : BisCommonSerializationOptions {
    public static readonly ParamSerializationOptions Defaults = new() {
        Indentation = 0,
        OrganizeEntries = true
    };
    
    public int Indentation { get; set; }
    public bool OrganizeEntries { get; set; } = true;
}

public interface IParamParsable<in Ctx, out Serializable> : IBisContextSerializable<Ctx, Serializable, ParamSerializationOptions>, IBisBinarizable where Ctx : ParserRuleContext where Serializable : IBisContextSerializable<Ctx, Serializable> {

    IBisBinarizable IBisSerializable<ParamSerializationOptions>.ReadString(StringBuilder builder,
        ParamSerializationOptions deserializationOptions) =>
        ReadString(builder);

    IBisBinarizable IBisSerializable.ReadString(StringBuilder builder) {
        var lexer = new ParamLexer(CharStreams.fromString(builder.ToString()));
        var tokens = new CommonTokenStream(lexer);
        var parser = new Generated.ParamLang.ParamParser(tokens);
        var funcName = nameof(Ctx)[0..^7];
        funcName = char.ToLower(funcName[0]) + funcName[1..];
        return (IBisBinarizable) FromParserContext(((Ctx)parser.GetType().GetMethod(funcName)!.Invoke(parser, null)!)!);
    }

    void IBisSerializable.WriteString(StringBuilder builder) =>
        WriteString(builder, ParamSerializationOptions.Defaults);
}
