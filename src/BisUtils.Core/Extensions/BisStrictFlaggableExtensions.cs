namespace BisUtils.Core.Extensions;

using Binarize.Flagging;

public static class BisStrictFlaggableExtensions
{
    public static bool HasStrictFlag<TFlag>(this IBisStrictFlaggable<TFlag> flaggable, TFlag flag) where TFlag : Enum =>
        flaggable.HasFlag(flag);

    public static void AddStrictFlag<TFlag>(this IBisStrictFlaggable<TFlag> flaggable, TFlag flag) where TFlag : Enum =>
        flaggable.AddFlag(flag);

    public static void RemoveStrictFlag<TFlag>(this IBisStrictFlaggable<TFlag> flaggable, TFlag flag)
        where TFlag : Enum =>
        flaggable.RemoveFlag(flag);

    public static IEnumerable<TFlag> GetStrictFlags<TFlag>(this IBisStrictFlaggable<TFlag> flaggable)
        where TFlag : Enum =>
        flaggable.GetFlags<TFlag>();
}
