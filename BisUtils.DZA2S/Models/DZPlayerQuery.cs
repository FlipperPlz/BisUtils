using BisUtils.DZA2S.Enumerations;

namespace BisUtils.DZA2S.Models;

public record DZPlayerQuery(
    
    ) : IDzQuery {

    private static readonly byte[] _sendMagic, _receiveMagic;

    public static byte[] GetSendMagic() => _sendMagic;
    public static byte[] GetReceiveMagic() => _receiveMagic;
    
    static DZPlayerQuery() {
        var bytes = new List<byte>();
        bytes.Add((byte) SteamQueryCode.PlayersCode);
        bytes.AddRange(IDzQuery.QueryHeader);
        _sendMagic = bytes.ToArray();
        _receiveMagic = new[] { (byte)SteamQueryCode.PlayersResponse };
    }   
}