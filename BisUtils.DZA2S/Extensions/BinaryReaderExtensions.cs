using BisUtils.DZA2S.Models;

namespace BisUtils.DZA2S.Extensions; 

public static class BinaryReaderExtensions {

    internal static bool ReadSteamHeader(this BinaryReader reader) => reader.ReadByte() == 0xFF
                                                                      && reader.ReadByte() == 0xFF
                                                                      && reader.ReadByte() == 0xFF
                                                                      && reader.ReadByte() == 0xFF;
    
    public static DZInfoQuery ReadDZInfoQuery(this BinaryReader reader) {
        var protocol = reader.ReadByte();
        var name = reader.ReadAsciiZ();
        var map = reader.ReadAsciiZ();
        var folder = reader.ReadAsciiZ();
        var game = reader.ReadAsciiZ();
        var id = reader.ReadInt16();
        var players = reader.ReadByte();
        var maxPlayers = reader.ReadByte();
        var bots = reader.ReadByte();
        var serverType = (SteamServerType) reader.ReadByte();
        var environmentType = (SteamEnvironmentType) reader.ReadByte();
        var visibilityType = (SteamSeverVisibility) reader.ReadByte();
        var vacProtected = reader.ReadByte() == 1;
        var version = reader.ReadAsciiZ();
        var extraDataFlag = (SteamExtraDataFlags) reader.ReadByte();
        short? port = null;
        ulong? steamId = null, gameId = null;
        short? spectatorPort = null;
        string? spectator = null, keywords = null;

        if (extraDataFlag.HasFlag(SteamExtraDataFlags.Port)) port = reader.ReadInt16();

        if (extraDataFlag.HasFlag(SteamExtraDataFlags.SteamId)) steamId = reader.ReadUInt64();

        if (extraDataFlag.HasFlag(SteamExtraDataFlags.Spectator)) {
            spectatorPort = reader.ReadInt16();
            spectator = reader.ReadAsciiZ();
        }

        if (extraDataFlag.HasFlag(SteamExtraDataFlags.Keywords)) keywords = reader.ReadAsciiZ();
        if (extraDataFlag.HasFlag(SteamExtraDataFlags.GameId)) gameId = reader.ReadUInt64();
        return new DZInfoQuery(protocol, name, map, folder, game, version, id, players, maxPlayers,
            bots, serverType, environmentType, visibilityType, extraDataFlag, vacProtected, gameId, steamId,
            keywords, spectator, spectatorPort, port);
    }

    public static DZPlayerQuery ReadDZPlayerQuery(this BinaryReader reader) {
        // var version = reader.ReadInt32();
        // var difficulty = reader.ReadInt32();
        // var dlcFlags = reader.ReadInt16();
        
        throw new NotImplementedException();
    }

}