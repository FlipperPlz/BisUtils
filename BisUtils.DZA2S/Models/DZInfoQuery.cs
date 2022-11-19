using System.IO;
using System.Text;
using BisUtils.DZA2S.Enumerations;

namespace BisUtils.DZA2S.Models;

//enums
[Flags]
public enum SteamSeverVisibility : byte {
    Public  = 0,
    Private = 1
}

[Flags]
public enum SteamServerType : byte {
    Dedicated = 0x64, //d
    NonDedicated = 0x6C, //l
    SourceTV = 0x70 //p
}

[Flags]
public enum SteamEnvironmentType : byte {
    Linux = 0x6C, //l
    Windows = 0x77, //w
    Mac = 0x6D, //m
    MacOsX = 0x6F //o
}

[Flags]
public enum SteamExtraDataFlags : byte
{
    GameId = 0x01,
    SteamId = 0x10,
    Keywords = 0x20,
    Spectator = 0x40,
    Port = 0x80
}

public record DZInfoQuery(
    byte                      ProtocolVersion,
    string                    Name,
    string                    Map,
    string                    Folder,
    string                    Game,
    string                    Version,
    short                     Identifier,
    byte                      PlayerCount,
    byte                      MaxPlayers,
    byte                      BotCount,
    SteamServerType           SteamServerType,
    SteamEnvironmentType      SteamEnvironmentType,
    SteamSeverVisibility      SteamSeverVisibility,
    SteamExtraDataFlags       SteamExtraData,
    bool                      VacSecured,
    ulong?                    GameId,
    ulong?                    SteamId,
    string?                   Keywords,
    string?                   Spectator,
    short?                    SpectatorPort,
    short?                    Port) : IDzQuery {
    private static readonly byte[] _sendMagic, _receiveMagic;

    public static byte[] GetSendMagic() => _sendMagic;
    public static byte[] GetReceiveMagic() => _receiveMagic;

    static DZInfoQuery() {
        var bytes = new List<byte>();
        bytes.Add((byte) SteamQueryCode.InfoCode);
        bytes.AddRange(Encoding.ASCII.GetBytes(IDzQuery.InfoQueryMessage));
        bytes.Add(0x00);
        _sendMagic = bytes.ToArray();
        _receiveMagic = new[] { (byte)SteamQueryCode.InfoResponse };
    }

}