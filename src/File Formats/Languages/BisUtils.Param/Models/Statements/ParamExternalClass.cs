namespace BisUtils.Param.Models.Statements;

using Core.IO;
using FResults;
using Options;
using Stubs;
using Stubs.Holders;

public interface IParamExternalClass : IParamStatement
{
    string ClassName { get; }
}

public class ParamExternalClass : ParamStatement, IParamExternalClass
{
    private string className = "";
    public string ClassName { get => className; set => className = value; }
    public override byte StatementId => 3;

    public ParamExternalClass(IParamFile? file, IParamStatementHolder? parent, string className) : base(file, parent) => ClassName = className;

    public ParamExternalClass(IParamFile? file, IParamStatementHolder? parent, BisBinaryReader reader, ParamOptions options) : base(file, parent, reader, options)
    {
        if (!Debinarize(reader, options))
        {
            throw new Exception(); //TODO: ERROR
        }
    }


    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        writer.WriteAsciiZ(className, options.Charset);
        return LastResult = Result.ImmutableOk();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, ParamOptions options) =>
        LastResult = reader.ReadAsciiZ(out className, options);

    public override Result Validate(ParamOptions options) => Result.Ok();

    public override Result ToParam(out string str, ParamOptions options)
    {
        str = $"class {ClassName};";
        return LastResult = Result.Ok();
    }

}
