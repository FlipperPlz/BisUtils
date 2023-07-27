namespace BisUtils.RVBank.Extensions;

using Core.IO;
using Model.Entry;
using Options;

public static class RVBankDataEntryExtensions
{
    public static bool InitializeFull(this IRVBankDataEntry ctx, long offset, BisBinaryReader reader, RVBankOptions options)
    {
        ctx.InitializeStreamOffset(offset);
        return ctx.InitializeBuffer(reader, options);
    }
}
