namespace BisUtils.Param.Models.Stubs;

using Core.Family;
using Core.IO;
using FResults;
using Options;

public interface IParamStatement : IParamElement, IFamilyChild
{
    IFamilyParent? IFamilyChild.Parent => ParentClass;
    IParamStatementHolder? ParentClass { get; }
}

public abstract class ParamStatement : ParamElement, IParamStatement
{
    public IParamStatementHolder? ParentClass { get; protected set; }

    protected ParamStatement(IParamFile? file, IParamStatementHolder? parent) : base(file) =>
        ParentClass = parent;

    protected ParamStatement(IParamFile? file, BisBinaryReader reader, ParamOptions options) : base(file, reader, options)
    {
    }

}
