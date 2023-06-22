namespace BisUtils.Core.Parsing.Errors;

using FResults.Reasoning;
using Lexer;

public class BisEndOfFileError : ErrorBase
{
    public static readonly BisEndOfFileError Instance = new();


    public override string? AlertName
    {
        get => "ReachedEOF";
        init => throw new NotSupportedException();
    }

    public override Type? AlertScope
    {
        get => null;
        init => throw new NotImplementedException();
    }

    public override string? Message
    {
        get => "The EOF was reached prematurely.";
        set => throw new NotSupportedException();
    }

    public static BisEndOfFileError CreateInstanceWithData(Dictionary<string, object> metadata) =>
        new() { Metadata = metadata };
}
