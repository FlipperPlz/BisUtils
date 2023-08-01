namespace BisUtils.RVBank.Extensions;

using Model.Stubs;

public static class RVBankEntryExtensions
{
    public static IEnumerable<IRVBankEntry> RetrieveDuplicateEntries(this IRVBankEntry ctx) => ctx.ParentDirectory.GetEntries<IRVBankEntry>().Where(it => it.EntryName == ctx.EntryName && it != ctx);
    public static T? RetrieveDuplicateEntry<T>(this T ctx) where T : IRVBankEntry =>
        ctx.ParentDirectory.GetEntries<T>().FirstOrDefault(it => it.EntryName == ctx.EntryName && EqualityComparer<T>.Default.Equals(it, ctx)) ;

    public static bool IsEmptyMeta(this IRVBankEntry ctx) =>
        ctx is { OriginalSize: 0, Offset: 0, TimeStamp: 0, DataSize: 0 };
    public static bool IsDummyEntry(this IRVBankEntry ctx) => IsEmptyMeta(ctx) && ctx.EntryName == "";
}
