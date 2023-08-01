namespace BisUtils.RVBank.Alerts.Exceptions;

using Model.Misc;

public class RVBankChecksumMismatch : Exception
{
    public RVBankDigest ExpectedDigest { get; }
    public RVBankDigest ActualDigest { get; }

    public RVBankChecksumMismatch(RVBankDigest expectedDigest, RVBankDigest actualDigest)
        : base("RVBank checksum mismatch.")
    {
        ExpectedDigest = expectedDigest;
        ActualDigest = actualDigest;
    }

    public override string ToString() => $"RVBank checksum mismatch. Expected: {ExpectedDigest}, Actual: {ActualDigest}";
}
