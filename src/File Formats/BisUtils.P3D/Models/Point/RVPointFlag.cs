namespace BisUtils.P3D.Models.Point;

[Flags]
public enum RVPointFlag : uint
{
    None = 0U,
    OnLand = 1U,
    UnderLand = 2U,
    AboveLand = 4U,
    KeepLand = 8U,
    LandMask = 15U,
    Decal = 256U,
    VDecal = 512U,
    DecalMask = 768U,
    NoLight = 16U,
    Ambient = 32U,
    FullLight = 64U,
    HalfLight = 128U,
    LightMask = 240U,
    NoFog = 4096U,
    SkyFog = 8192U,
    FogMask = 12288U,
    UserMask = 16711680U,
    UserStep = 65536U,
    SpecialMask = 251658240U,
    SpecialHidden = 16777216U,
    AllFlags = 268383231U
}
