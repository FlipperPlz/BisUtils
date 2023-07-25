namespace BisUtils.Param.Factories;

using Core.IO;
using FResults;
using Microsoft.Extensions.Logging;
using Models;
using Models.Literals;
using Models.Stubs;
using Models.Stubs.Holders;
using Options;

public static class ParamLiteralFactory
{

    public static Result ReadLiteral(BisBinaryReader reader,
        ParamOptions options, out IParamLiteral? literal, IParamFile file, IParamLiteralHolder parent, ILogger? logger)
    {
        literal = (options.LastLiteralId = reader.ReadByte()) switch
        {
            0 => new ParamString(reader, options, file, parent, logger),
            1 => new ParamFloat(reader, options, file, parent, logger),
            2 => new ParamInt(reader, options, file, parent, logger),
            3 => new ParamArray(reader, options, file, parent, logger),
            _ => null
        };//TODO: Get IDs From Types
        if (literal is null)
        {
            return Result.Fail($"Unknown Literal ID '{options.LastLiteralId}'.");
        }

        return literal.LastResult ?? Result.Ok();
    }
}
