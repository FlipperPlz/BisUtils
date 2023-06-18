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

    public ParamDelete(BisBinaryReader reader, ParamOptions options) : base(reader, options)
    {
    }


    public Result LocateDeleteTarget(out IParamExternalClass? clazz)
    {
        //TODO

        clazz = null;
        return LastResult = Result.Fail($"Could not locate target '{DeleteTargetName}'of delete statement");
    }
    public override Result Binarize(BisBinaryWriter writer, ParamOptions options) => throw new NotImplementedException();

    public override Result Debinarize(BisBinaryReader reader, ParamOptions options) => throw new NotImplementedException();

    public override Result Validate(ParamOptions options) => throw new NotImplementedException();

    public override Result ToParam(out string str, ParamOptions options) => throw new NotImplementedException();
}
