namespace BisUtils.Param.Models;

using Core.Family;
using Core.IO;
using FResults;
using Options;
using Statements;
using Stubs;

public interface IParamFile : IParamClass, IFamilyNode
{
    string FileName { get; }

    string IParamExternalClass.ClassName => FileName;

    IFamilyParent? IFamilyChild.Parent => null;

    IParamStatementHolder? IParamStatement.ParentClass => null;

    IParamStatementHolder? IParamStatementHolder.ParentClass => null;

    string? IParamClass.InheritedClassname => null;

    Result IParamClass.LocateParamParent(out IParamExternalClass? clazz)
    {
        clazz = null;
        return Result.ImmutableOk();
    }
}

public class ParamFile : ParamElement, IParamFile
{
    public string FileName { get; set; } = string.Empty;
    public List<IParamStatement> Statements { get; set; } = new();

    public ParamFile(string fileName, List<IParamStatement>? statements = null) : base(null)
    {
        FileName = fileName;
        Statements = statements ?? new List<IParamStatement>();
    }

    public ParamFile(BisBinaryReader reader, ParamOptions options) : base(null, reader, options)
    {
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options) => throw new NotImplementedException();

    public override Result Debinarize(BisBinaryReader reader, ParamOptions options) => throw new NotImplementedException();

    public override Result Validate(ParamOptions options) => throw new NotImplementedException();

    public override Result ToParam(out string str, ParamOptions options) => ((IParamClass)this).GetStatements(out str, options);
}
