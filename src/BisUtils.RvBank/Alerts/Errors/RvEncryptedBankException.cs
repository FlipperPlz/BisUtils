namespace BisUtils.RvBank.Alerts.Errors;

public class RvEncryptedBankException : IOException
{
    public RvEncryptedBankException() : base("BisUtils is currently configured to prohibit encrypted PBOs.")
    {
    }
}
