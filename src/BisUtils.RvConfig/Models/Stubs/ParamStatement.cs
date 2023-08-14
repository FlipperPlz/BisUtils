namespace BisUtils.RvConfig.Models.Stubs;

using Core.IO;
using FResults;
using Holders;
using Microsoft.Extensions.Logging;
using Options;

public interface IParamStatement : IParamElement
{
    IParamStatementHolder ParentClass { get; set; }
    byte StatementId { get; }
    public void SyncToContext(IParamStatementHolder holder);
}

public abstract class ParamStatement : ParamElement, IParamStatement
{
    protected ParamStatement(IParamFile file, IParamStatementHolder parent, ILogger? logger) : base(file, logger) =>
        ParentClass = parent;

    protected ParamStatement(  BisBinaryReader reader,
        ParamOptions options, IParamFile file, IParamStatementHolder parent, ILogger? logger) : base( reader, options, file, logger) =>
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
