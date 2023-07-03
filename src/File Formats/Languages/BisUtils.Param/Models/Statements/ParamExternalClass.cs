namespace BisUtils.Param.Models.Statements;

using Core.IO;
using FResults;
using Options;
using Stubs;

public interface IParamExternalClass : IParamStatement
{
    string ClassName { get; }
}

public class ParamExternalClass : ParamStatement, IParamExternalClass
{
    private string className = "";
    public string ClassName { get => className; set => className = value; }

    public ParamExternalClass(IParamFile? file, IParamStatementHolder? parent, string className) : base(file, parent) => ClassName = className;

    public ParamExternalClass(BisBinaryReader reader, ParamOptions options) : base(reader, options)
    {
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        writer.WriteAsciiZ(className, options.Charset);
        return LastResult = Result.ImmutableOk();
    }

    public override Result Debinarize(BisBinaryReader reader, ParamOptions options) =>
        LastResult = reader.ReadAsciiZ(out className, options);

    public override Result Validate(ParamOptions options) => Result.Ok();

    public override Result ToParam(out string str, ParamOptions options)
    {
        str = $"class {ClassName};";
        return LastResult = Result.Ok();
    }

}
