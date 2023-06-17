namespace BisUtils.Bank.Model.Stubs;

using System.Diagnostics;
using BisUtils.Core.IO;
using FResults;
using Options;
using Utils;

public interface IPboEntry : IPboVFSEntry
{
    PboEntryMime EntryMime { get; }
    int OriginalSize { get; }
    int Offset { get; }
    int TimeStamp { get; }
    int DataSize { get; }

    bool IsEmptyMeta()
    {
        var watch = Stopwatch.StartNew();
        var ret =
            OriginalSize == 0 &&
            Offset == 0 &&
            TimeStamp == 0 &&
            DataSize == 0;
        watch.Stop();
        Console.WriteLine($"(IPboEntry::IsEmptyMeta) Execution Time: {watch.ElapsedMilliseconds} ms");
        return ret;
    }

    bool IsDummyEntry()
    {
        var watch = Stopwatch.StartNew();
        var ret = IsEmptyMeta() && EntryName == "";
        watch.Stop();
        Console.WriteLine($"(IPboEntry::IsDummyEntry) Execution Time: {watch.ElapsedMilliseconds} ms");
        return ret;
    }

}

public abstract class PboEntry : PboVFSEntry
{
    public PboEntryMime EntryMime { get; set; } = PboEntryMime.Decompressed;
    public int OriginalSize { get; set; }
    public int Offset { get; set;  }
    public int TimeStamp { get; set; }
    public int DataSize { get; set; }

    protected PboEntry(
        IPboFile? file,
        IPboDirectory? parent,
        string fileName,
        PboEntryMime mime,
        int originalSize,
        int offset,
        int timeStamp,
        int dataSize
    ) : base(file, parent, fileName)
    {
        EntryMime = mime;
        OriginalSize = originalSize;
        Offset = offset;
        TimeStamp = timeStamp;
        DataSize = dataSize;
    }

    protected PboEntry(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
    }

    public bool IsEmptyMeta()
    {
#if DEBUG
        var watch = Stopwatch.StartNew();
#endif
        var ret =
            OriginalSize == 0 &&
            Offset == 0 &&
            TimeStamp == 0 &&
            DataSize == 0;
#if DEBUG
        watch.Stop();
        Console.WriteLine($"(PboEntry::IsEmptyMeta) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif
        return ret;
    }

    public bool IsDummyEntry()
    {
#if DEBUG
        var watch = Stopwatch.StartNew();
#endif

        var ret = IsEmptyMeta() && EntryName == "";
#if DEBUG
        watch.Stop();
                Console.WriteLine($"(PboEntry::IsDummyEntry) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif

        return ret;
    }

}
