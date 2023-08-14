namespace BisUtils.DzServerQuery.Models;

public interface IDzQuery {
    public static readonly byte[] QueryHeader = new byte[] {0xFF, 0xFF, 0xFF, 0xFF};
    public const string InfoQueryMessage = "Source Engine Query";
    public static byte[] GetSendMagic() => throw new NotSupportedException();
    public static byte[] GetReceiveMagic() => throw new NotSupportedException();
}
