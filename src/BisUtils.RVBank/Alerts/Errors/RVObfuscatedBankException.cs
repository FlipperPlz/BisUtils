namespace BisUtils.RVBank.Alerts.Errors;

public class RVObfuscatedBankException : IOException
{
    public RVObfuscatedBankException() : base("BisUtils is currently configured to prohibit obfuscated PBOs.")
    {
    }
}
