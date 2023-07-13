namespace BisUtils.P3D.Models.Face;

[Flags]
public enum RVFaceFlag
{
    Default = 0,
    ShadowOff = 16,
    MergingOff = 16777216,
    ZBiasLow = 256,
    ZBiasMid = 512,
    ZBiasHigh = 768,
    LightningBoth = 32,
    LightningPosition = 128,
    LightningFlat = 2097152,
    LightningReversed = 1048576
}
