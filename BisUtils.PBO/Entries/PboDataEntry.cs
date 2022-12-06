using BisUtils.Core;
using BisUtils.PBO.Builders;

namespace BisUtils.PBO.Entries; 

public class PboDataEntry : PboEntry {

    private bool _queuedToDelete;
    internal bool RespectOffsets = false;
    
    public virtual byte[] EntryData {
        get => EntryParent.GetEntryData(this);
        set => EntryParent.OverwriteEntryData(this, value, EntryMagic == PboEntryMagic.Compressed);
    }

    public ulong OriginalSize {
        get => Reserved1;
        set => Reserved1 = value;
    }

    public ulong TimeStamp {
        get => Reserved3;
        set => Reserved3 = value;
    }
    
    public ulong PackedSize {
        get => Reserved4;
        set => Reserved4 = value;
    }

    public ulong BinaryOffset {
        get => Reserved2;
        set => value = Reserved2;
    }
    
    
    

    public ulong EntryDataStartOffset; //Relative to pbo data block
    public ulong EntryDataStopOffset; //Relative to pbo data block

    public PboDataEntry(IPboFile entryParent) : base(entryParent) {
        
    }

    public override void WriteBinary(BinaryWriter writer) {
        writer.WriteAsciiZ(EntryName);
        writer.Write((int) EntryMagic);
        writer.Write((int) OriginalSize);
        writer.Write((int) Reserved3);
        writer.Write((int) TimeStamp);
        writer.Write((int) PackedSize);
    }

    internal void ReinitializeOffsets() {
        EntryDataStartOffset = 0;
        
        foreach (var ent in EntryParent.GetPboEntries()) {
            if(ent is not PboDataEntry dataEntry) continue;
            if (ent == this) break;
            EntryDataStartOffset += dataEntry.PackedSize;
        }

        EntryDataStopOffset = EntryDataStartOffset + PackedSize;
    }

    public override int CompareTo(PboEntry? other) {
        throw new NotImplementedException();
    }

    public void QueueDeletion() => _queuedToDelete = true;

    public bool IsQueuedForDeletion() => _queuedToDelete;
}