using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Literals;

namespace BisUtils.Parsers.ParamParser.Factories; 

public class RapLiteralFactory {
    public static IRapArrayEntry Create(Generated.ParamLang.ParamParser.LiteralOrArrayContext ctx) {
        if (ctx.literalArray() is not null) return RapArray.FromContext(ctx.literalArray());
        if (ctx.literal() is not null) return (IRapArrayEntry) Create(ctx.literal());
        throw new Exception();
    }

    public static IRapLiteral Create(Generated.ParamLang.ParamParser.LiteralContext ctx) {
        if (ctx.literalString() is not null) return RapString.FromContext(ctx.literalString());
        if (ctx.literalFloat() is not null) return RapFloat.FromContext(ctx.literalFloat());
        if (ctx.literalInteger() is not null) return RapInteger.FromContext(ctx.literalInteger());
        throw new Exception();
    }
}