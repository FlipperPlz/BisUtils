namespace BisUtils.DzServerQuery.Models;

using Enumerations;

public record DzRulesQuery(
    byte ProtocolVersion,
    byte Difficulty,
    short DlcFlags,
    IEnumerable<DzServerMod> ServerMods
) : IDzQuery {


    private static readonly byte[] SendMagic, ReceiveMagic;

    public static byte[] GetSendMagic() => SendMagic;
    public static byte[] GetReceiveMagic() => ReceiveMagic;

    static DzRulesQuery() {
        SendMagic = new[] { (byte)DzSteamQueryCode.RulesCode }.Concat(IDzQuery.QueryHeader).ToArray();
        ReceiveMagic = new[] { (byte)DzSteamQueryCode.RulesResponse };
    }
}
