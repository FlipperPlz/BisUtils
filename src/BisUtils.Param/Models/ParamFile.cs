namespace BisUtils.Param.Models;

using Core.Extensions;
using Core.IO;
using FResults;
using FResults.Extensions;
using Lexer;
using Options;
using Parse;
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


    public static ParamFile ReadParamFile(string fileName, Stream stream, ParamOptions options)
    {
        using var reader = new BisBinaryReader(stream);
        if (reader.ReadByte() != 0 ||
            reader.ReadByte() != 'r' ||
            reader.ReadByte() != 'a' ||
            reader.ReadByte() != 'P')
        {
            reader.Dispose();
            var streamReader = new StreamReader(stream);
            var lexer = new ParamLexer(streamReader.ReadToEnd());
            streamReader.Dispose();
            var result = ParamParser.Instance.Parse(out var node, lexer);
            if (node is null)
            {
                result.Throw();
            }
            node.LastResult = result;
            return node;
        }
        reader.BaseStream.Seek(-4, SeekOrigin.Current);
        return new ParamFile(fileName, reader, options);
    }

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
