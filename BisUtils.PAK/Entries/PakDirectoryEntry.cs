using BisUtils.Core.Serialization;
using BisUtils.PAK.Enums;
using BisUtils.PAK.Interfaces;

namespace BisUtils.PAK.Entries; 

public class PakDirectoryEntry : PakEntry, IPakEnumerable {
    public List<PakEntry> Children { get; }

    public PakDirectoryEntry(IPakEnumerable? parent) : base(PakEntryType.Directory, parent) {
        Children = new List<PakEntry>();
    }

    public override IBisBinarizable ReadBinary(BinaryReader reader) {
        base.ReadBinary(reader);
        var entryCount = reader.ReadInt32();
        for (var e = 0; e < entryCount; e++) Children.Add(ReadPakEntry(reader, this));
        return this;
    }

    public override void WriteBinary(BinaryWriter writer) {
        base.WriteBinary(writer);
        writer.WriteInt32BE(Children.Count);
        foreach (var child in Children) child.WriteBinary(writer);
    }

}