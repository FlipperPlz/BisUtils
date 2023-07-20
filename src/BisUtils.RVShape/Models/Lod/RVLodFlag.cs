namespace BisUtils.RVShape.Models.Lod;

using Point;

public enum RVLodFlag
{
    //Land Mask 0xf
    OnLand = 0x1,
    UnderLand = 0x2,
    AboveLand = 0x4,
    KeepLand = 0x8,
    //Decal Mask 0x300
    Decal = 0x100,
    VerticalDecal = 0x200,
    //Light Mask 0xf0
    NoLight = 0x10,
    AmbientLight = 0x20,
    FullLight = 0x40,
    HalfLight = 0x80,
    //Fog Mask 0x3000
    NoFog = 0x1000,
    SkyFog = 0x2000,
    //Special Mask 0xf000000
    SpecialHidden = 0x1000000,
    //User Mask 0xff0000
    UserStep = 0x010000,

    AllFlags = OnLand | UnderLand | AboveLand | KeepLand | Decal | VerticalDecal |
               NoLight | FullLight | HalfLight | AmbientLight | NoFog | SkyFog | 0xf000000 | 0xff0000


}
