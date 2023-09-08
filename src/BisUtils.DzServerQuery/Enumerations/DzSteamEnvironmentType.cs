namespace BisUtils.DzServerQuery.Enumerations;

[Flags]
public enum DzSteamEnvironmentType : byte {
    Linux = 0x6C, //l
    Windows = 0x77, //w
    Mac = 0x6D, //m
    MacOsX = 0x6F //o
}
