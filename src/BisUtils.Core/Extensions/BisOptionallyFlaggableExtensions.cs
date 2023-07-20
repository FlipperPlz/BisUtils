namespace BisUtils.Core.Extensions;

using Binarize.Flagging;

public static class BisOptionallyFlaggableExtensions
{
    public static bool IsFlaggable<TFlags>(this IBisOptionallyFlaggable<TFlags> flaggable) =>
        flaggable.Flags is not null;
}
