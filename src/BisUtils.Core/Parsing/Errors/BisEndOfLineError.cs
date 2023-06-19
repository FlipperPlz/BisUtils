namespace BisUtils.Core.Parsing.Errors;

using FResults.Reasoning;
using Lexer;

public class BisEndOfLineError : ErrorBase
{
    public static readonly BisEndOfLineError Instance = new();

    public override string? AlertName
    {
        get => "ReachedEOL";
        init => throw new NotSupportedException();
    }

    public override Type? AlertScope
    {
        get => typeof(BisLexer);
        init => throw new NotSupportedException();
    }

    public override string? Message
    {
        get => "The EOL was reached prematurely.";
        set => throw new NotSupportedException();
    }

    public static BisEndOfLineError CreateInstanceWithData(Dictionary<string, object> metadata) =>
        new() { Metadata = metadata };
}
