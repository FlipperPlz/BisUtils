namespace BisUtils.Core.Binarize.Flagging;

public static class BisFlagUtils
{
    public static TFlag CreateFlagsFor<TFlag>(params dynamic[] iValue) where TFlag : struct, Enum
    {
        dynamic result = default(TFlag);
        var index = 0;
        for (; index < iValue.Length; index++)
        {
            var value = iValue[index];
            result |= value;
        }

        return result;
    }
}
