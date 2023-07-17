namespace BisUtils.Param.Models;

using Core.Extensions;
using Core.IO;
using FResults;
using Options;
using Statements;
using Stubs;

public interface IParamFile : IParamClass
{
    string FileName { get; set; }
}

public class ParamFile : ParamClass, IParamFile
{
    public string FileName { get => ClassName; set => ClassName = value; }

    public ParamFile(string fileName, List<IParamStatement>? statements = null) : base(null, null, fileName, null, statements)
    {
        FileName = fileName;
        Statements = statements ?? new List<IParamStatement>();
    }

    public ParamFile(string fileName, BisBinaryReader reader, ParamOptions options) : base(null, null, reader, options)
    {
        FileName = fileName;
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options) => throw new NotImplementedException();

    public override Result Validate(ParamOptions options) => throw new NotImplementedException();

    private new Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
     //TODO: //
     return Result.Ok();
    }
}
