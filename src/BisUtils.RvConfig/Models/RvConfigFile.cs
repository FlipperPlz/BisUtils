namespace BisUtils.RvConfig.Models;

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

public interface IRvConfigFile : IParamClass
{
    string FileName { get; set; }
}

public class RvConfigFile : ParamClass, IRvConfigFile
{
    public string FileName { get => ClassName; set => ClassName = value; }

    public RvConfigFile(string fileName, List<IParamStatement> statements, ILogger? logger) : base( fileName, null, statements, null!, null!, logger)
    {
        FileName = fileName;
        Statements = statements;
    }

    public RvConfigFile(string fileName, BisBinaryReader reader, ParamOptions options, ILogger? logger) : base(reader, options, null!, null!, logger)
    {
        FileName = fileName;
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RvConfigFile()
    {

    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options) => throw new NotImplementedException();


    public static RvConfigFile? ReadParamFile(string fileName, Stream stream, ParamOptions options, ILogger? logger)
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
            // var lexer = new RvConfigLexer(
            //     reader,
            //     options.Charset,
            //     StepperDisposalOption.Dispose,
            //     logger,
            //     stringStart: 0L
            // );
            // return RvConfigParser.Instance.Parse(lexer, logger);
        }
        reader.BaseStream.Seek(-4, SeekOrigin.Current);
        return new RvConfigFile(fileName, reader, options, logger);
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
