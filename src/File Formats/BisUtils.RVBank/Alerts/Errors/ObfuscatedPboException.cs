namespace BisUtils.RVBank.Alerts.Errors;

public class ObfuscatedPboException : IOException
{
    public ObfuscatedPboException() : base("BisUtils is currently configured to prohibit obfuscated PBOs.")
    {
    }
}
