using System.Text;
using BisUtils.Core;

namespace BisUtils.PBO.Entries; 

public sealed class PboVersionEntry : BasePboEntry {
    public Dictionary<string, string> Metadata { get; set; }
    
    public PboVersionEntry(PboFile entryParent, Dictionary<string, string>? metadata = null) : base(entryParent) {
        Metadata = metadata ?? new Dictionary<string, string>();
    }

    public new ulong CalculateMetaLength() {
        var offset = 21;
        foreach (var prop in Metadata) {
            offset += Encoding.UTF8.GetBytes(prop.Key).Length + 1;
            offset += Encoding.UTF8.GetBytes(prop.Value).Length + 1;
        }

        offset++;

        return (ulong) offset;
    } 
    
    public new IBisSerializable ReadBinary(BinaryReader reader) {
        EntryName = reader.ReadAsciiZ();
        EntryMagic = (PboEntryMagic) reader.ReadInt32();
        Reserved1 = reader.ReadUInt32();
        Reserved2 = reader.ReadUInt32();
        Reserved3 = reader.ReadUInt32();
        DataLength = reader.ReadUInt32();

        string propertyName;
        while ((propertyName = reader.ReadAsciiZ()) != string.Empty) {
            var propertyValue = reader.ReadAsciiZ();

            Metadata.Add(propertyName, propertyValue);
        }
        
        return this;
    }
    
    public new void WriteBinary(BinaryWriter writer) {
        if (EntryMagic is PboEntryMagic.Undefined) throw new Exception("Cannot write undefined entry.");
        writer.Write(EntryName);
        writer.Write((int) EntryMagic);
        writer.Write((int) Reserved1);
        writer.Write((int) Reserved2);
        writer.Write((int) Reserved3);
        writer.Write((int) DataLength);

        foreach (var metaProperty in Metadata) {
            writer.WriteAsciiZ(metaProperty.Key);
            writer.WriteAsciiZ(metaProperty.Value);
        }
        
        writer.WriteAsciiZ();
    }
    
    public override int CompareTo(BasePboEntry? other) {
        throw new NotImplementedException();
    }
}