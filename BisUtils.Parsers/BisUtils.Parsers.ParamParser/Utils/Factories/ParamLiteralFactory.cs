using BisUtils.Parsers.ParamParser.Literals;

namespace BisUtils.Parsers.ParamParser.Utils.Factories; 

public static class ParamLiteralFactory {
    public static IParamLiteral Create(Generated.ParamLang.ParamParser.LiteralOrArrayContext ctx) {
        if (ctx.literalArray() is not null) return new ParamLiteralArray(ctx.literalArray());
        if (ctx.literal() is not null) return Create(ctx.literal());
        throw new NotSupportedException();
    }
    
    public static IParamLiteral Create(Generated.ParamLang.ParamParser.LiteralContext ctx) {
        if (ctx.literalString() is not null) return new ParamLiteralString(ctx.literalString());
        if(!double.TryParse(ctx.GetText(), out var number)) throw new Exception("Failed to read number");

        return number switch {
            >= int.MinValue and <= int.MaxValue when (int)number is { } integer => new ParamLiteralInteger(integer),
            >= float.MinValue and <= float.MaxValue when (float)number is { } fl => new ParamLiteralFloat(fl),
            _ => throw new NotSupportedException("Doubles are not supported by BisUtils")
        };
    }
}