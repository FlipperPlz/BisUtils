namespace BisUtils.Bank.Alerts.Errors;

using FResults.Reasoning;
using Model;

public class EmptyPboPropertyNameWarning : ErrorBase
{
    public static readonly EmptyPboPropertyNameWarning Instance = new();

    public static EmptyPboPropertyNameWarning CreateInstanceWithData(Dictionary<string, object> metadata) =>
        new() { Metadata = metadata };

    private EmptyPboPropertyNameWarning()
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

}
