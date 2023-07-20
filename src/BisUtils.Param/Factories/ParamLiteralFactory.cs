namespace BisUtils.Param.Factories;

using Core.IO;
using FResults;
using Models;
using Models.Literals;
using Models.Stubs;
using Models.Stubs.Holders;
using Options;

public static class ParamLiteralFactory
{

    public static Result ReadLiteral(IParamFile? file, IParamLiteralHolder? parent, BisBinaryReader reader,
        ParamOptions options, out IParamLiteral? literal)
    {
        literal = (options.LastLiteralId = reader.ReadByte()) switch
        {
            0 => new ParamString(file, parent, reader, options),
            1 => new ParamFloat(file, parent, reader, options),
            2 => new ParamInt(file, parent, reader, options),
            3 => new ParamArray(file, parent, reader, options),
            _ => null
        };//TODO: Get IDs From Types
        if (literal is null)
        {
            return Result.Fail($"Unknown Literal ID '{options.LastLiteralId}'.");
        }

        return literal.LastResult ?? Result.Ok();
    }
}
