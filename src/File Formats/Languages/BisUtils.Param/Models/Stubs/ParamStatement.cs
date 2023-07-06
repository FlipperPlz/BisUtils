namespace BisUtils.Param.Models.Stubs;

using Core.Family;
using Core.IO;
using FResults;
using Holders;
using Options;
using Statements;

public interface IParamStatement : IParamElement
{
    IParamStatementHolder? ParentClass { get; }
    byte StatementId { get; }

}

public abstract class ParamStatement : ParamElement, IParamStatement
{
    public abstract byte StatementId { get; }
    public IParamStatementHolder? ParentClass { get; protected set; }

    protected ParamStatement(IParamFile? file, IParamStatementHolder? parent) : base(file) =>
        ParentClass = parent;

    protected ParamStatement(IParamFile? file, IParamStatementHolder? parent, BisBinaryReader reader, ParamOptions options) : base(file, reader, options) =>
        ParentClass = parent;

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        if (options.WriteStatementId)
        {
            writer.Write(StatementId);
        }
        return Result.Ok();
    }
}
