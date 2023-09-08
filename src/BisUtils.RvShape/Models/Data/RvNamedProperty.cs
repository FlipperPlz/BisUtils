namespace BisUtils.RvShape.Models.Data;

public interface IRvNamedProperty
{
    string PropertyName { get; set; }
    string PropertyValue { get; set; }
}

public class RvNamedProperty : IRvNamedProperty
{
    public string PropertyName { get; set; }
    public string PropertyValue { get; set; }

    public RvNamedProperty(string propertyName, string propertyValue)
    {
        PropertyName = propertyName;
        PropertyValue = propertyValue;
    }
}
