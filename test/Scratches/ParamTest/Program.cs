using System;
using System.Collections.Generic;

namespace BisUtils.Core.Binarize.Flagging;

public interface IBisFlaggable<TFlags>
{
    TFlags Flags { get; set; }
}

public static class BisFlaggableExtensions
{
    public static bool HasFlag<TFlag>(this IBisFlaggable<TFlag> flaggable, TFlag flag) where TFlag : struct
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
        where TFlag : struct
    {
        dynamic flagValue = flag;
        dynamic flagsValue = flaggable.Flags;
        flaggable.Flags = flagsValue & ~flagValue;
    }

    public static IEnumerable<TFlag> GetFlags<TFlag>(this IBisFlaggable<TFlag> flaggable)where TFlag : struct, Enum
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



public class Foo
{
    public static void Main()
    {
        var impl = new TestImpl();
        Console.WriteLine((int) impl.Flags); // out: 0

        impl.AddFlag(Test.LightningBoth);
        Console.WriteLine((int) impl.Flags); //out: 32


        impl.AddFlag(Test.ZBiasHigh);
        Console.WriteLine((int)impl.Flags);  //out: 800

    }
}

class TestImpl : IBisFlaggable<Test>
{
    public Test Flags { get; set; }
}
enum Test : int
{
    Default = 0,
    ShadowOff = 16,
    LightningBoth = 32,
    LightningPosition = 128,
    ZBiasLow = 256,
    ZBiasMid = 512,
    ZBiasHigh = 768,
}
