namespace BisUtils.RvBank.Extensions;

using Model.Stubs;

public static class RvBankEntryExtensions
{
    public static IEnumerable<IRvBankEntry> RetrieveDuplicateEntries(this IRvBankEntry ctx) => ctx.ParentDirectory.GetEntries<IRvBankEntry>().Where(it => it.EntryName == ctx.EntryName && it != ctx);
    public static T? RetrieveDuplicateEntry<T>(this T ctx) where T : IRvBankEntry =>
        ctx.ParentDirectory.GetEntries<T>().FirstOrDefault(it => it.EntryName == ctx.EntryName && EqualityComparer<T>.Default.Equals(it, ctx)) ;

    public static bool IsEmptyMeta(this IRvBankEntry ctx) =>
        ctx is { OriginalSize: 0, Offset: 0, TimeStamp: 0, DataSize: 0 };
    public static bool IsDummyEntry(this IRvBankEntry ctx) => IsEmptyMeta(ctx) && ctx.EntryName == "";
}
