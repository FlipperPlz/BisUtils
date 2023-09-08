namespace BisUtils.RvBank.Alerts.Errors;

public class RvObfuscatedBankException : IOException
{
    public RvObfuscatedBankException() : base("BisUtils is currently configured to prohibit obfuscated PBOs.")
    {
    }
}
