using System.Text;
using BisUtils.Core;

namespace BisUtils.PBO.Entries; 

public enum PboEntryMagic {
    Version      = 0x56657273 , 
    Compressed   = 0x43707273,
    Encrypted    = 0x456e6372,
    Decompressed = 0x00000000,
    
    Undefined    = -1
}

public abstract class BasePboEntry : IBisSerializable, IComparable<BasePboEntry> {
    public readonly PboFile EntryParent;
    
    public PboEntryMagic EntryMagic = PboEntryMagic.Undefined;
    public string EntryName { get; set; } = string.Empty;
    protected ulong Reserved1 { get; set; }
    protected ulong Reserved2 { get; set; }
    protected ulong Reserved3 { get; set; }
    protected ulong Reserved4 { get; set; }

    protected BasePboEntry(PboFile entryParent) {
        EntryParent = entryParent;
    }

    public virtual IBisSerializable ReadBinary(BinaryReader reader) {
        EntryName = reader.ReadAsciiZ();
        EntryMagic = (PboEntryMagic) reader.ReadInt32();
        Reserved1 = reader.ReadUInt32();
        Reserved2 = reader.ReadUInt32();
        Reserved3 = reader.ReadUInt32();
        Reserved4 = reader.ReadUInt32();
        
        return this;
    }

    public virtual void WriteBinary(BinaryWriter writer) {
        if (EntryMagic is PboEntryMagic.Undefined) throw new Exception($"Cannot write undefined entry. {EntryName}");
        writer.Write(EntryName);
        writer.Write((int) EntryMagic);
        writer.Write((int) Reserved1);
        writer.Write((int) Reserved2);
        writer.Write((int) Reserved3);
        writer.Write((int) Reserved4);
    }

    public virtual ulong CalculateMetaLength() => (ulong) (Encoding.UTF8.GetBytes(EntryName).Length + 21);
    
    public static BasePboEntry ReadPboEntry(PboFile pboFile, BinaryReader reader) {
        var startPos = reader.BaseStream.Position;

        var entryName = reader.ReadAsciiZ();
        var entryMagic = (PboEntryMagic) reader.ReadInt32();
        ulong reserved1 = reader.ReadUInt32(), reserved2 = reader.ReadUInt32(), reserved3 = reader.ReadUInt32(), reserved4 = reader.ReadUInt32();
        Console.WriteLine($"Debug: Reading {entryMagic.ToString()} Entry \nName:\"{entryName}\" \nReserved 1-4: ({reserved1}, {reserved2}, {reserved3}, {reserved4})\n");
        switch (entryMagic) {
            case PboEntryMagic.Version: {
                if (entryName is not "") throw new Exception("Version entries should not be named.");
                if (reserved1 != 0 || reserved2 != 0 || reserved3 != 0 || reserved4 != 0)
                    throw new Exception("Data was found in the reserved section of a version entry");
                reader.BaseStream.Position = startPos;
                
                return (BasePboEntry) new PboVersionEntry(pboFile).ReadBinary(reader);
            }
            case PboEntryMagic.Compressed: {
                reader.BaseStream.Position = startPos;
                return (BasePboEntry) new PboDataEntry(pboFile).ReadBinary(reader);
            }
            case PboEntryMagic.Encrypted: throw new NotImplementedException();
            case PboEntryMagic.Decompressed: {
                if (entryName == string.Empty && reserved1 == 0 && reserved2 == 0 && reserved3 == 0 && reserved4 == 0)
                    return new PboDummyEntry(pboFile);
                reader.BaseStream.Position = startPos;
                return (BasePboEntry) new PboDataEntry(pboFile).ReadBinary(reader); 
            }

            case PboEntryMagic.Undefined:
            default:throw new ArgumentOutOfRangeException(entryMagic.ToString());
        }

        
    }

    

    public abstract int CompareTo(BasePboEntry? other);
}
