namespace BisUtils.RvBank.Alerts.Exceptions;

using Model.Misc;

public class RvBankChecksumMismatchException : Exception
{
    public RvBankDigest ExpectedDigest { get; }
    public RvBankDigest ActualDigest { get; }

    public RvBankChecksumMismatchException(RvBankDigest expectedDigest, RvBankDigest actualDigest)
        : base("RVBank checksum mismatch.")
    {
        ExpectedDigest = expectedDigest;
        ActualDigest = actualDigest;
    }

    public override string ToString() => $"RVBank checksum mismatch. Expected: {ExpectedDigest}, Actual: {ActualDigest}";
}
