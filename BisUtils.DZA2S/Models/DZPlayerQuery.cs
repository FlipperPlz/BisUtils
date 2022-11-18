using BisUtils.DZA2S.Enumerations;

namespace BisUtils.DZA2S.Models;

public record DZPlayerQuery(
    
    ) : IDzQuery {
    public static byte[] Magic { get; }

    static DZPlayerQuery() {
        var bytes = new List<byte>();
        bytes.Add((byte) SteamQueryCode.PlayersCode);
        bytes.AddRange(IDzQuery.QueryHeader);
        Magic = bytes.ToArray();
    }   
}