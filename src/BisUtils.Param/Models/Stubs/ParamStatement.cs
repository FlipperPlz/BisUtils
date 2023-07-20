namespace BisUtils.Param.Models.Stubs;

using Core.IO;
using FResults;
using Holders;
using Options;

public interface IParamStatement : IParamElement
{
    IParamStatementHolder ParentClass { get; set; }
    byte StatementId { get; }
    public void SyncToContext(IParamStatementHolder holder);
}

public abstract class ParamStatement : ParamElement, IParamStatement
{
    protected ParamStatement(IParamFile file, IParamStatementHolder parent) : base(file) =>
        ParentClass = parent;

    protected ParamStatement(IParamFile file, IParamStatementHolder parent, BisBinaryReader reader,
        ParamOptions options) : base(file, reader, options) =>
        ParentClass = parent;

    public abstract byte StatementId { get; }
    public IParamStatementHolder ParentClass { get; set; }

    public void SyncToContext(IParamStatementHolder holder)
    {
        ParentClass = holder;
        ParamFile = holder.ParamFile;
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        if (options.WriteStatementId)
        {
            writer.Write(StatementId);
        }

        return Result.Ok();
    }
}
