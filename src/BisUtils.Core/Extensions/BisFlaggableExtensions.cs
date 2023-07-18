namespace BisUtils.Core.Extensions;

using Binarize.Flagging;

public static class BisFlaggableExtensions
{
    public static bool HasFlag<TFlag>(this IBisFlaggable<TFlag> flaggable, TFlag flag) where TFlag : struct, Enum
    {
        dynamic flagValue = flag;
        dynamic flagsValue = flaggable.Flags;

        return (flagsValue & flagValue) == flagValue;
    }


    public static void AddFlag<TFlag>(this IBisFlaggable<TFlag> flaggable, TFlag flag)
        where TFlag : struct, Enum
    {
        dynamic flagValue = flag;
        dynamic flagsValue = flaggable.Flags;
        flaggable.Flags = flagsValue | flagValue;
    }

    public static void RemoveFlag<TFlag>(this IBisFlaggable<TFlag> flaggable, TFlag flag)
        where TFlag : struct, Enum
    {
        dynamic flagValue = flag;
        dynamic flagsValue = flaggable.Flags;
        flaggable.Flags = flagsValue & ~flagValue;
    }

    public static IEnumerable<TFlag> GetFlags<TFlag>(this IBisFlaggable<TFlag> flaggable) where TFlag : struct, Enum
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
