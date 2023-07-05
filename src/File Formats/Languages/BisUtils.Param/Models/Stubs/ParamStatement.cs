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

    public static Result DebinarizeStatement(IParamFile? file, IParamStatementHolder parent, BisBinaryReader reader, ParamOptions options, out IParamStatement? statement)
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

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        if (options.WriteStatementId)
        {
            writer.Write(StatementId);
        }
        return Result.Ok();
    }
}
