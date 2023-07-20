namespace BisUtils.RVBank.Alerts.Errors;

public class RVEncryptedBankException : IOException
{
    public RVEncryptedBankException() : base("BisUtils is currently configured to prohibit encrypted PBOs.")
    {
    }
}
