namespace BisUtils.DZServerQuery.Enumerations;

public enum DzSteamQueryCode : byte
{
    InfoCode = 0x54,
    PlayersCode = 0x55,
    RulesCode = 0x56,
    MasterCode = 0x31,
    InfoResponse = 0x49,
    PlayersResponse = 0x44,
    RulesResponse = 0x45,
    MasterResponse = 0x66,

    ChallengeResponse = 0x41
}
