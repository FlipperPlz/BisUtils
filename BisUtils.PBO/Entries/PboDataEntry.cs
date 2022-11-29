namespace BisUtils.PBO.Entries; 

public class PboDataEntry : BasePboEntry {
    
    public MemoryStream EntryData {
        get => EntryParent.ReadEntryData(this);
        set => EntryParent.OverwriteEntryData(this, value, EntryMagic == PboEntryMagic.Compressed);
    }


    public ulong OriginalSize {
        get => Reserved1;
        set => Reserved1 = value;
    }

    public ulong TimeStamp => Reserved3;
    public ulong PackedSize {
        get => DataLength;
        set => DataLength = value;
    }

    public ulong EntryDataStartOffset; //Relative to pbo data block
    public ulong EntryDataStopOffset; //Relative to pbo data block

    public PboDataEntry(PboFile entryParent) : base(entryParent) {
        
    }

    internal void ReinitializeOffsets() {
        EntryDataStartOffset = 0;
        
        foreach (var ent in EntryParent.PboEntries) {
            if(ent is not PboDataEntry) continue;
            if (ent == this) break;
            EntryDataStartOffset += ent.DataLength;
        }

        EntryDataStopOffset = EntryDataStartOffset + DataLength;
    }

    public override int CompareTo(BasePboEntry? other) {
        throw new NotImplementedException();
    }
}