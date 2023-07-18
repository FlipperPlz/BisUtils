namespace BisUtils.Core.Binarize.Flagging;

public interface IBisFlaggable<TFlags>
{
    TFlags Flags { get; set; }
}
