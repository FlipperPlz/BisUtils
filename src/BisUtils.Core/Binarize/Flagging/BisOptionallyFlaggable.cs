namespace BisUtils.Core.Binarize.Flagging;

public interface IBisOptionallyFlaggable<TFlags>
{
    TFlags? Flags { get; set; }
}
