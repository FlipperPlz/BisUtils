namespace BisUtils.RVBank.Alerts.Errors;

public class EncryptedPboException : IOException
{
    public EncryptedPboException() : base("BisUtils is currently configured to prohibit encrypted PBOs.")
    {
    }
}
