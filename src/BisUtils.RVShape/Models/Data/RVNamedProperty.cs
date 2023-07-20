namespace BisUtils.RVShape.Models.Data;

public interface IRVNamedProperty
{
    string PropertyName { get; set; }
    string PropertyValue { get; set; }
}

public class RVNamedProperty : IRVNamedProperty
{
    public string PropertyName { get; set; }
    public string PropertyValue { get; set; }

    public RVNamedProperty(string propertyName, string propertyValue)
    {
        PropertyName = propertyName;
        PropertyValue = propertyValue;
    }
}
