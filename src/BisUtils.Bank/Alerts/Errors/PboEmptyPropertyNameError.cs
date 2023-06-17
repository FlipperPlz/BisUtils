namespace BisUtils.Bank.Alerts.Errors;

using FResults.Reasoning;
using Model;

public class PboEmptyPropertyNameError : ErrorBase
{
    public static readonly PboEmptyPropertyNameError Instance = new();

    private PboEmptyPropertyNameError()
    {
    }

    public override string? AlertName
    {
        get => "EmptyPboProperty";
        init => throw new NotSupportedException();
    }

    public override Type? AlertScope
    {
        get => typeof(PboFile);
        init => throw new NotSupportedException();
    }

    public override string? Message
    {
        get => "Pbo Properties cannot be empty as this is what signifies the last property";
        set => throw new NotSupportedException();
    }

    public static PboEmptyPropertyNameError CreateInstanceWithData(Dictionary<string, object> metadata) =>
        new() { Metadata = metadata };
}
