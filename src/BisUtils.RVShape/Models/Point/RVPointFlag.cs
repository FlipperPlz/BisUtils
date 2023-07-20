namespace BisUtils.RVShape.Models.Point;

[Flags]
public enum RVPointFlag : uint
{
    None = 0U,
    OnLand = 0x1,
    UnderLand = 0x2,
    AboveLand = 0x4,
    KeepLand = 0x8,
    LandMask = 0xf,
    Decal = 0x100,
    VDecal = 0x200,
    DecalMask = 0x300,
    NoLight = 0x10,
    Ambient = 0x20,
    FullLight = 0x40,
    HalfLight = 0x80,
    LightMask = 0xf0,
    NoFog = 0x1000,
    SkyFog = 0x2000,
    FogMask = 0x3000,
    UserMask = 0xff0000,
    UserStep = 0x02000000,
    SpecialMask = 0xf000000,
    SpecialHidden = 0x1000000,
}
