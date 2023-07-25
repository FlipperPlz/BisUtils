namespace BisUtils.Param.Factories;

using Core.IO;
using FResults;
using Microsoft.Extensions.Logging;
using Models;
using Models.Statements;
using Models.Stubs;
using Models.Stubs.Holders;
using Options;

public static class ParamStatementFactory
{
    public static Result ReadStatement(BisBinaryReader reader, ParamOptions options, out IParamStatement? statement, IParamFile file, IParamStatementHolder parent, ILogger? logger)
    {
        statement = (options.LastStatementId = reader.ReadByte()) switch
        {
            0 => new ParamClass(reader, options, file, parent, logger),
            1 or 2 or 5 => new ParamVariable(reader, options, file, parent, logger),
            3 => new ParamExternalClass(reader, options, file, parent, logger),
            4 => new ParamDelete(reader, options, file, parent, logger),
            _ => null
        };
        if (statement is null)
        {
            return Result.Fail($"Unknown Literal ID '{options.LastStatementId}'.");
        }

        return statement.LastResult ?? Result.Ok();
    }
}
