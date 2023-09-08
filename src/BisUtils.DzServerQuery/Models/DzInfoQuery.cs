namespace BisUtils.DzServerQuery.Models;

using System.Text;
using Core.Binarize.Flagging;
using Enumerations;

public record DzInfoQuery(
    byte ProtocolVersion,
    string Name,
    string Map,
    string Folder,
    string Game,
    string Version,
    short Identifier,
    byte PlayerCount,
    byte MaxPlayers,
    byte BotCount,
    SteamServerType SteamServerType,
    DzSteamEnvironmentType SteamEnvironmentType,
    SteamSeverVisibility SteamSeverVisibility,
    bool VacSecured,
    ulong? GameId,
    ulong? SteamId,
    string? Keywords,
    string? Spectator,
    short? SpectatorPort,
    short? Port) : IBisFlaggable<DzInfoQueryFlags> {
    private static readonly byte[] SendMagic, ReceiveMagic;
    public DzInfoQueryFlags Flags { get; set; }

    public static byte[] GetSendMagic() => SendMagic;
    public static byte[] GetReceiveMagic() => ReceiveMagic;

    static DzInfoQuery() {
        SendMagic = new byte[] { (byte)DzSteamQueryCode.InfoCode }.Concat(Encoding.ASCII.GetBytes(IDzQuery.InfoQueryMessage)).Append(byte.MinValue).ToArray();
        ReceiveMagic = new byte[] { (byte)DzSteamQueryCode.InfoResponse };
    }

}
