using System.Text;
using BisUtils.Core;

namespace BisUtils.PBO.Entries;

public struct PboProperty {
    public string PropertyName { get; set; }
    public string PropertyValue { get; set; }
}

public sealed class PboVersionEntry : BasePboEntry {

    public List<PboProperty> Metadata { get; set; }
    
    public PboVersionEntry(IPboFile entryParent, List<PboProperty>? metadata = null) : base(entryParent) {
        Metadata = metadata ?? new List<PboProperty>();
        EntryMagic = PboEntryMagic.Version;
    }

    public void AddMetadataProperty(string key, string value, bool syncPbo = false) {
        EntryParent.DeSyncStream();
        Metadata.Add(new PboProperty() {
            PropertyName = key,
            PropertyValue = value
        });
        
        if (syncPbo) EntryParent.SyncToStream();
    }

    public override ulong CalculateMetaLength() {
        var offset = 21;
        foreach (var prop in Metadata) {
            offset += Encoding.UTF8.GetBytes(prop.PropertyName).Length + 1;
            offset += Encoding.UTF8.GetBytes(prop.PropertyValue).Length + 1;
        }

        offset++;

        return (ulong) offset;
    } 
    
    public override IBisSerializable ReadBinary(BinaryReader reader) {
        EntryName = reader.ReadAsciiZ();
        EntryMagic = (PboEntryMagic) reader.ReadInt32();
        Reserved1 = reader.ReadUInt32();
        Reserved2 = reader.ReadUInt32();
        Reserved3 = reader.ReadUInt32();
        Reserved4 = reader.ReadUInt32();

        string propertyName;
        while ((propertyName = reader.ReadAsciiZ()) != string.Empty) {
            var propertyValue = reader.ReadAsciiZ();

            AddMetadataProperty(propertyName, propertyValue, false);
        }
        
        
        return this;
    }
    
    public override void WriteBinary(BinaryWriter writer) {
        if (EntryMagic is PboEntryMagic.Undefined) throw new Exception("Cannot write undefined entry.");
        writer.Write(EntryName);
        writer.Write((int) EntryMagic);
        writer.Write((int) Reserved1);
        writer.Write((int) Reserved2);
        writer.Write((int) Reserved3);
        writer.Write((int) Reserved4);

        foreach (var metaProperty in Metadata) {
            writer.WriteAsciiZ(metaProperty.PropertyName);
            writer.WriteAsciiZ(metaProperty.PropertyValue);
        }
        
        writer.WriteAsciiZ();
    }
    
    public override int CompareTo(BasePboEntry? other) {
        throw new NotImplementedException();
    }
}