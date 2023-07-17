namespace BisUtils.Param.Models.Statements;

using Core.Extensions;
using Core.IO;
using FResults;
using Options;
using Stubs;
using Stubs.Holders;

public interface IParamExternalClass : IParamStatement
{
    string ClassName { get; set; }
}

public class ParamExternalClass : ParamStatement, IParamExternalClass
{
    public string ClassName { get; set; } = null!;
    public override byte StatementId => 3;

    public ParamExternalClass(IParamFile? file, IParamStatementHolder? parent, string className) : base(file, parent) => ClassName = className;

    public ParamExternalClass(IParamFile? file, IParamStatementHolder? parent, BisBinaryReader reader, ParamOptions options) : base(file, parent, reader, options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        writer.WriteAsciiZ(ClassName, options.Charset);
        return LastResult = Result.ImmutableOk();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        LastResult = reader.ReadAsciiZ(out var className, options);
        ClassName = className;
        return LastResult;
    }

    public override Result Validate(ParamOptions options) => Result.Ok();


    public override Result WriteParam(out string value, ParamOptions options)
    {
        value = $"class {ClassName};";
        return LastResult = Result.Ok();
    }
}
