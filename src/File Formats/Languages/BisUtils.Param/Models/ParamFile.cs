namespace BisUtils.Param.Models;

using Core.Family;
using Core.IO;
using FResults;
using Options;
using Statements;
using Stubs;
using Stubs.Holders;

public interface IParamFile : IParamClass
{
    string FileName { get; }

    string IParamExternalClass.ClassName => FileName;

    IParamStatementHolder? IParamStatement.ParentClass => null;

    IParamStatementHolder? IParamStatementHolder.ParentClass => null;

    string? IParamClass.InheritedClassname => null;

    Result IParamClass.LocateParamParent(out IParamExternalClass? clazz)
    {
        clazz = null;
        return Result.ImmutableOk();
    }
}

public class ParamFile : ParamClass, IParamFile
{
    public string FileName { get; set; } = string.Empty;

    public ParamFile(string fileName, List<IParamStatement>? statements = null) : base(null, null, fileName, null, statements)
    {
        FileName = fileName;
        Statements = statements ?? new List<IParamStatement>();
    }

    public ParamFile(string fileName, BisBinaryReader reader, ParamOptions options) : base(null, null, reader, options)
    {
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options) => throw new NotImplementedException();

    public override Result Validate(ParamOptions options) => throw new NotImplementedException();

    public Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
     //TODO: //
     return Result.Ok();
    }
}
