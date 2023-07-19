namespace BisUtils.RVBank.Extensions;

using Core.Parsing;
using Model;
using Model.Stubs;

public static class RVBankDirectoryExtensions
{

    public static IRVBankDirectory? GetDirectory(this IRVBankDirectory ctx, string name) =>
        ctx.Directories.FirstOrDefault(e => e.EntryName == name);

    public static IRVBankDirectory CreateDirectory(this IRVBankDirectory ctx, string name, IRVBank node)
    {
        var split = name.Split('\\', 2);
        IRVBankDirectory ret;

        if (split[0].Length == 0)
        {
            return ctx;
        }

        if (GetDirectory(ctx, split[0]) is { } i)
        {
            if (split[1].Length == 0)
            {
                return i;
            }

            if (!ctx.PboEntries.Contains(i))
            {
                ctx.PboEntries.Add(i);
            }

            ret = i.CreateDirectory(split[1], node);

            return ret;
        }

        var directory = new RVBankDirectory(node, ctx, new List<IRVBankEntry>(), RVPathUtilities.GetFilename(split[0]));
        ctx.PboEntries.Add(directory);
        if (split[1].Length == 0)
        {
            return directory;
        }

        ret = directory.CreateDirectory(split[1], node);

        return ret;
    }
}
