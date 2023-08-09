namespace BisUtils.RvBank.Alerts.Errors;

using FResults.Reasoning;
using Model;

public class RvBankEmptyPropertyNameError : ErrorBase
{
    public static readonly RvBankEmptyPropertyNameError Instance = new();

    private RvBankEmptyPropertyNameError()
    {
    }

    public override string? AlertName
    {
        get => "EmptyPboProperty";
        init => throw new NotSupportedException();
    }

    public override Type? AlertScope
    {
        get => typeof(RvBank);
        init => throw new NotSupportedException();
    }

    public override string? Message
    {
        get => "Pbo Properties cannot be empty as this is what signifies the last property";
        set => throw new NotSupportedException();
    }

    public static RvBankEmptyPropertyNameError CreateInstanceWithData(Dictionary<string, object> metadata) =>
        new() { Metadata = metadata };
}
