namespace BisUtils.RvShape.Models.Lod;

public enum RvFaceHint : uint
{
    ClipNone = 0,
    ClipFront = 1,
    ClipBack = 2,
    ClipLeft = 4,
    ClipRight = 8,
    ClipBottom = 16,
    ClipTop = 32,
    ClipUser = 0x40,
    ClipAll = ClipFront | ClipBack | ClipLeft | ClipRight | ClipBottom | ClipTop,

    ClipLandMask = 0xf00,
    ClipLandOn = 0x100,
    ClipLandUnder = ClipLandOn * 2,
    ClipLandAbove = ClipLandOn * 4,
    ClipLandKeep = ClipLandOn * 8,

    ClipDecalMask = 0x3000,
    ClipDecalNormal = 0x1000,
    ClipDecalVertical = ClipDecalNormal * 2,

    ClipFogMask = 0xc000,
    ClipFogDisable = 0x4000,
    ClipFogSky = ClipFogDisable * 2,

    ClipLightMask = 0xf0000,
    ClipLightStep=0x10000,
    ClipLightLine = ClipLightStep * 8,

    ClipUserMask = 0xff00000,
    ClipUserStep = 0x100000,
    MaxUserValue = 0xff,

    AllHints = ClipLandMask | ClipDecalMask | ClipFogMask | ClipLightMask | ClipUserMask
}
