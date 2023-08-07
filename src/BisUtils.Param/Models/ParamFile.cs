namespace BisUtils.Param.Models;

using Core.Extensions;
using Core.IO;
using FResults;
using FResults.Extensions;
using Lexer;
using Microsoft.Extensions.Logging;
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

    public ParamFile(string fileName, List<IParamStatement> statements, ILogger? logger) : base( fileName, null, statements, null!, null!, logger)
    {
        FileName = fileName;
        Statements = statements;
    }

    public ParamFile(string fileName, BisBinaryReader reader, ParamOptions options, ILogger? logger) : base(reader, options, null!, null!, logger)
    {
        FileName = fileName;
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options) => throw new NotImplementedException();


    public static ParamFile? ReadParamFile(string fileName, Stream stream, ParamOptions options, ILogger? logger)
    {
        stream.Seek(0, SeekOrigin.Begin);
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        memory.Seek(0, SeekOrigin.Begin);

        using var reader = new BisBinaryReader(memory);
        if (reader.ReadByte() != 0 ||
            reader.ReadByte() != 'r' ||
            reader.ReadByte() != 'a' ||
            reader.ReadByte() != 'P')
        {
            var content = options.Charset.GetString(memory.ToArray());
            Console.WriteLine(content);
            var lexer = new ParamLexerOld(content);
            var result = ParamParser.Instance.Parse(out var node, lexer, logger);
            return node;
        }
        reader.BaseStream.Seek(-4, SeekOrigin.Current);
        return new ParamFile(fileName, reader, options, logger);
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
