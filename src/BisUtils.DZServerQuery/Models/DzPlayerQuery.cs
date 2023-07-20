namespace BisUtils.DZServerQuery.Models;

using Enumerations;

public class DzPlayerQuery
{
    private static readonly byte[] SendMagic, ReceiveMagic;

    public static byte[] GetSendMagic() => SendMagic;
    public static byte[] GetReceiveMagic() => ReceiveMagic;

    static DzPlayerQuery() {
        SendMagic = new[] { (byte) DzSteamQueryCode.PlayersCode }.Concat(IDzQuery.QueryHeader).ToArray();
        ReceiveMagic = new[] { (byte)DzSteamQueryCode.PlayersResponse };
    }
}
