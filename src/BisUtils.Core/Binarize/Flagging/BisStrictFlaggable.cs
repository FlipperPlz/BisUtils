namespace BisUtils.Core.Binarize.Flagging;

public interface IBisStrictFlaggable<TFlags> : IBisFlaggable where TFlags : Enum
{
}
