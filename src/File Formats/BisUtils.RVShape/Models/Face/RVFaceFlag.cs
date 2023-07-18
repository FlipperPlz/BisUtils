namespace BisUtils.RVShape.Models.Face;

[Flags]
public enum RVFaceFlag : uint
{
    LightMask = 0x3000a7,
    NoLight = 0x1,
    AmbientLight = 0x2,
    FullLight = 0x4,
    DualSidedLight = 0x20,
    SkyLight = 0x80,
    ReverseLight = 0x100000,
    FlatLight = 0x200000,
    ShadowMask = 0x18,
    IsShadow = 0x8,
    NoShadow = 0x10,
    ZBiasMask = 0x300,
    ZBiasStep = 0x100,
    FsMask = 0xf0000,
    FsBeginFan = 0x10000,
    FsBeginStrip = 0x20000,
    FsContinueFan = 0x40000,
    FsContinueStrip = 0x80000,
    UserMask = 0xfe000000,
    UserStep = 0x02000000,
    UserShift = 25,
    DisableTextureMerge = 0x1000000
}
