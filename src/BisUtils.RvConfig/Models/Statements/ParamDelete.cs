namespace BisUtils.RvConfig.Models.Statements;

using System.Text;
using Core.Extensions;
using Core.IO;
using Extensions;
using FResults;
using Microsoft.Extensions.Logging;
using Options;
using Stubs;
using Stubs.Holders;

public interface IParamDelete : IParamStatement
{
    string DeleteTargetName { get; }
    Result LocateDeleteTarget(out IParamExternalClass? clazz);
}

public class ParamDelete : ParamStatement, IParamDelete
{
    public override byte StatementId => 4;
    public string DeleteTargetName { get; set; } = null!;

    public ParamDelete(string target, IParamFile file, IParamStatementHolder parent, ILogger? logger) : base(file, parent, logger) =>
        DeleteTargetName = target;

    public ParamDelete(BisBinaryReader reader, ParamOptions options, IParamFile file, IParamStatementHolder parent, ILogger? logger) : base(reader, options, file, parent, logger)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public Result LocateDeleteTarget(out IParamExternalClass? clazz)
    {
        clazz = ParentClass.LocateAnyClass(DeleteTargetName);
        return LastResult = clazz is null
            ? Result.Fail($"Could not locate target '{DeleteTargetName}' of delete statement")
            : Result.Ok();
    }


    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        var result = base.Binarize(writer, options);
        writer.WriteAsciiZ(DeleteTargetName, options);
        return result;
    }

    public sealed override Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        var result = reader.ReadAsciiZ(out var target, options);
        DeleteTargetName = target;
        return result;
    }

    public override Result Validate(ParamOptions options)
    {
        if (!options.MissingDeleteTargetIsError)
        {
            return Result.Ok();
        }

        var result = LocateDeleteTarget(out _);
        return result.IsFailed
            ? result //TODO: Check class access level
            : Result.Ok();
    }


    public override Result WriteParam(ref StringBuilder builder, ParamOptions options)
    {
        builder.Append("delete ").Append(DeleteTargetName).Append(';');
        return Result.Ok();
    }
}
