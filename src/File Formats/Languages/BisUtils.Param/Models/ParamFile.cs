namespace BisUtils.Param.Models;

using Core.Extensions;
using Core.IO;
using FResults;
using FResults.Extensions;
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

    public ParamFile(string fileName, List<IParamStatement> statements ) : base(null!, null!, fileName, null, statements)
    {
        FileName = fileName;
        Statements = statements;
    }

    public ParamFile(string fileName, BisBinaryReader reader, ParamOptions options) : base(null!, null!, reader, options)
    {
        FileName = fileName;
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options) => throw new NotImplementedException();


    private new Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        LastResult = Result.FailIf
        (
            reader.ReadByte() != 0 ||
            reader.ReadByte() != 'r' ||
            reader.ReadByte() != 'a' ||
            reader.ReadByte() != 'P',
            "Invalid param magic."
        );
        if (LastResult.IsFailed)
        {
            return LastResult;
        }

        LastResult.WithReasons
        (
            Result.FailIf
            (
                reader.ReadInt32() != 0 ||
                reader.ReadInt32() != 8,
                "Invalid param version."
            ).Reasons
        );
        if (LastResult.IsFailed)
        {
            return LastResult;
        }

        var enumOffset = reader.ReadInt32();
        LastResult.WithReasons(base.Debinarize(reader, options).Reasons);
        //TODO: read enums

        return LastResult;
    }
}
