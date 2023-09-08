namespace BisUtils.RvConfig.Models.Statements;

using System.Text;
using Core.Extensions;
using Core.IO;
using FResults;
using Microsoft.Extensions.Logging;
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

    public ParamExternalClass(string className, IRvConfigFile file, IParamStatementHolder parent, ILogger? logger) : base(file, parent, logger) => ClassName = className;

    public ParamExternalClass(BisBinaryReader reader, ParamOptions options, IRvConfigFile file, IParamStatementHolder parent, ILogger? logger) : base(reader, options, file, parent, logger)
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


    public override Result WriteParam(ref StringBuilder builder, ParamOptions options)
    {
        builder.Append("class ").Append(ClassName).Append(';');
        return LastResult = Result.Ok();
    }
}
