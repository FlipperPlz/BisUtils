namespace BisUtils.RVBank.Alerts.Errors;

using FResults.Reasoning;
using Model;

public class RVBankEmptyPropertyNameError : ErrorBase
{
    public static readonly RVBankEmptyPropertyNameError Instance = new();

    private RVBankEmptyPropertyNameError()
    {
    }

    public override string? AlertName
    {
        get => "EmptyPboProperty";
        init => throw new NotSupportedException();
    }

    public override Type? AlertScope
    {
        get => typeof(RVBank);
        init => throw new NotSupportedException();
    }

    public override string? Message
    {
        get => "Pbo Properties cannot be empty as this is what signifies the last property";
        set => throw new NotSupportedException();
    }

    public static RVBankEmptyPropertyNameError CreateInstanceWithData(Dictionary<string, object> metadata) =>
        new() { Metadata = metadata };
}
