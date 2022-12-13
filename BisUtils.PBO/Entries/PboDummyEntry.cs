namespace BisUtils.PBO.Entries; 

public sealed class PboDummyEntry : PboEntry {
    public PboDummyEntry(PboFile entryParent) : base(entryParent) {
        EntryMagic = PboEntryMagic.Decompressed;
    }

    public override int CompareTo(PboEntry? other) {
        throw new NotImplementedException();
    }
}