using System.Text;
using BisUtils.DZA2S.Enumerations;

namespace BisUtils.DZA2S.Models; 

public record DZRulesQuery(
    byte                           ProtocolVersion, 
    byte                           Difficulty, 
    short                           DLCFlags,
    List<DZRulesQuery.ServerMod>   ServerMods
) : IDzQuery {
    public record ServerMod(
        string    ModName,
        long      ModID,
        int       ModHash,
        bool      DLC);
    
    private static readonly byte[] _sendMagic, _receiveMagic;

    public static byte[] GetSendMagic() => _sendMagic;
    public static byte[] GetReceiveMagic() => _receiveMagic;
    
    static DZRulesQuery() {
        var bytes = new List<byte>();
        bytes.Add((byte) SteamQueryCode.RulesCode);
        bytes.AddRange(IDzQuery.QueryHeader);
        _sendMagic = bytes.ToArray();
        _receiveMagic = new[] { (byte)SteamQueryCode.RulesResponse };
    }
}