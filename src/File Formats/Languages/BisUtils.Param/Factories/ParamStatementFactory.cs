namespace BisUtils.Param.Factories;

using Core.IO;
using FResults;
using Models;
using Models.Statements;
using Models.Stubs;
using Models.Stubs.Holders;
using Options;

public static class ParamStatementFactory
{
    public static Result ReadStatement(IParamFile? file, IParamStatementHolder parent, BisBinaryReader reader, ParamOptions options, out IParamStatement? statement)
    {
        statement = (options.LastStatementId = reader.ReadByte()) switch
        {
            0 => new ParamClass(file, parent, reader, options),
            1 or 2 or 5 => new ParamVariable(file, parent, reader, options),
            3 => new ParamExternalClass(file, parent, reader, options),
            4 => new ParamDelete(file, parent, reader, options),
            _ => null
        };
        if (statement is null)
        {
            return Result.Fail($"Unknown Literal ID '{options.LastStatementId}'.");
        }

        return statement.LastResult ?? Result.Ok();
    }
}
