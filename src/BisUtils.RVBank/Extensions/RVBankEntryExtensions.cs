namespace BisUtils.RVBank.Extensions;

using Model.Stubs;

public static class RVBankEntryExtensions
{
    public static IEnumerable<IRVBankEntry> RetrieveDuplicateEntries(this IRVBankEntry ctx) => ctx.ParentDirectory.GetEntries<IRVBankEntry>().Where(it => it.EntryName == ctx.EntryName && it != ctx);
}
