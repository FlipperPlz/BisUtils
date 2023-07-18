namespace BisUtils.Core.Extensions;

using System.Globalization;
using Binarize.Flagging;

public static class BisFlaggableExtensions
{
    public static bool HasFlag<TFlag>(this IBisFlaggable flaggable, TFlag flag) where TFlag : Enum
    {
        var flagValue = Convert.ToInt32(flag, CultureInfo.CurrentCulture);
        return (flaggable.Flags & flagValue) == flagValue;
    }

    public static void AddFlag<TFlag>(this IBisFlaggable flaggable, TFlag flag) where TFlag : Enum => flaggable.Flags |= Convert.ToInt32(flag, CultureInfo.CurrentCulture);

    public static void RemoveFlag<TFlag>(this IBisFlaggable flaggable, TFlag flag) where TFlag : Enum => flaggable.Flags &= ~Convert.ToInt32(flag, CultureInfo.CurrentCulture);

    public static IEnumerable<TFlag> GetFlags<TFlag>(this IBisFlaggable flaggable) where TFlag : Enum
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

