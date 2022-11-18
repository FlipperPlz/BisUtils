using BisUtils.DZA2S.Enumerations;

namespace BisUtils.DZA2S.Models;

public record DZPlayerQuery(
    
    ) : IDzQuery {

    private static readonly byte[] _magic;
    public static byte[] GetMagic() => _magic;
    static DZPlayerQuery() {
        var bytes = new List<byte>();
        bytes.Add((byte) SteamQueryCode.PlayersCode);
        bytes.AddRange(IDzQuery.QueryHeader);
        _magic= bytes.ToArray();
    }   
}