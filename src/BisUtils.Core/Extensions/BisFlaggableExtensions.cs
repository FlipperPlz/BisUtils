namespace BisUtils.Core.Extensions;

using Binarize.Flagging;

public static class BisFlaggableExtensions
{
    public static bool HasFlag<TFlags, TFlag>(this IBisFlaggable<TFlags> flaggable, TFlag flag) where TFlags : struct where TFlag : struct, Enum
    {
        dynamic flagValue = flag;
        dynamic flagsValue = flaggable.Flags;

        return (flagsValue & flagValue) == flagValue;
    }

    public static void AddFlag<TFlags, TFlag>(this IBisFlaggable<TFlags> flaggable, TFlag flag)
        where TFlags : struct where TFlag : struct, Enum
    {
        dynamic flagValue = flag;
        dynamic flagsValue = flaggable.Flags;
        flaggable.Flags = flagsValue | flagValue;
    }

    public static void RemoveFlag<TFlags, TFlag>(this IBisFlaggable<TFlags> flaggable, TFlag flag)
        where TFlags : struct where TFlag : struct, Enum
    {
        dynamic flagValue = flag;
        dynamic flagsValue = flaggable.Flags;
        flaggable.Flags = flagsValue & ~flagValue;
    }

    public static IEnumerable<TFlag> GetFlags<TFlags, TFlag>(this IBisFlaggable<TFlags> flaggable) where TFlags : struct where TFlag : struct, Enum
    {
        var flags = Enum.GetValues(typeof(TFlag));
        foreach (TFlag iFlag in flags)
        {
            if (flaggable.HasFlag(iFlag))
            {
                yield return iFlag;
            }
        }
    }
}
