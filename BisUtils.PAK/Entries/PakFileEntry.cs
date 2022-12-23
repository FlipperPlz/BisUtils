using BisUtils.Core.Serialization;
using BisUtils.PAK.Enums;
using BisUtils.PAK.Interfaces;

namespace BisUtils.PAK.Entries; 

public class PakFileEntry : PakEntry {
    public int Offset { get; set; }
    public int PackedSize { get; set; }
    public int OriginalSize { get; set; }
    public int CyclicRedundancyCheck { get; set; }
    public PakCompressionType CompressionType { get; set; }
    public PakCompressionLevel CompressionLevel { get; set; }


    public PakFileEntry(IPakEnumerable? parent) : base(PakEntryType.File, parent) {
    }

    public override IBisBinarizable ReadBinary(BinaryReader reader) {
        base.ReadBinary(reader);
        Offset = reader.ReadInt32();
        PackedSize = reader.ReadInt32();
        OriginalSize = reader.ReadInt32();
        CyclicRedundancyCheck = reader.ReadInt32();
        CompressionType = (PakCompressionType) reader.ReadByte();
        CompressionLevel = (PakCompressionLevel) reader.ReadByte();
        reader.BaseStream.Seek(6, SeekOrigin.Current);
        return this;
    }

    public override void WriteBinary(BinaryWriter writer) {
        base.WriteBinary(writer);
        writer.Write(Offset);
        writer.Write(PackedSize);
        writer.Write(OriginalSize);
        writer.Write(CyclicRedundancyCheck);
        writer.Write((byte) CompressionType);
        writer.Write((byte) CompressionLevel);
    }
}