namespace BisUtils.Param.Models.Statements;

using Core.IO;
using FResults;
using Options;
using Stubs;

public interface IParamDelete : IParamStatement
{
    string DeleteTargetName { get; }
    Result LocateDeleteTarget(out IParamExternalClass? clazz);
}

public class ParamDelete : ParamStatement, IParamDelete
{
    public string DeleteTargetName { get; set; } = "";

    public ParamDelete(IParamFile? file, IParamStatementHolder? parent, string target) : base(file, parent) =>
        DeleteTargetName = target;

    public ParamDelete(IParamFile file, BisBinaryReader reader, ParamOptions options) : base(file, reader, options)
    {
        if (!Debinarize(reader, options))
        {
            throw new Exception(); //TODO: ERROR
        }
    }

    public Result LocateDeleteTarget(out IParamExternalClass? clazz)
    {
        clazz = null; //TODO
        return LastResult = Result.Fail($"Could not locate target '{DeleteTargetName}' of delete statement");
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        writer.WriteAsciiZ(DeleteTargetName, options);
        return Result.Ok();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        var result = reader.ReadAsciiZ(out var target, options);
        DeleteTargetName = target;
        return result;
    }

    public override Result Validate(ParamOptions options) => throw new NotImplementedException(); //TODO: LocateDeleteTarget

    public override Result ToParam(out string str, ParamOptions options)
    {
        str = $"delete {DeleteTargetName};";
        return Result.Ok();
    }
}
