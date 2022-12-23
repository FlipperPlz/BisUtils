using BisUtils.Core.Serialization;
using BisUtils.PAK.Enums;
using BisUtils.PAK.Interfaces;

namespace BisUtils.PAK.Entries; 

public abstract class PakEntry : IBisBinarizable {
    public readonly PakEntryType EntryType;
    public readonly IPakEnumerable? EntryParent;
    public string EntryName { get; set; } = null!; //Length <= byte.MaxValue

    public string GetPath() {
        if (EntryParent is PakDirectoryEntry parentDirectory) return $"{parentDirectory.GetPath()}\\{EntryName}";
        return EntryName;
    }
    
    protected PakEntry(PakEntryType entryType, IPakEnumerable? parent) {
        EntryType = entryType;
        EntryParent = parent;
    }

    public virtual IBisBinarizable ReadBinary(BinaryReader reader) {
        if (reader.ReadByte() != (byte) EntryType) throw new Exception($"Expected {EntryType}, instead got opposite.");
        var nameLength = reader.ReadByte();
        EntryName = new string(reader.ReadChars(nameLength));

        return this;
    }
    
    public virtual void WriteBinary(BinaryWriter writer) {
        if (EntryName.Length > byte.MaxValue) throw new Exception("Entry name too long!");
        writer.Write((byte) EntryType);
        writer.Write((byte) EntryName.Length);
        writer.Write(EntryName.ToCharArray());
    }

    public bool IsDirectory() => EntryType is PakEntryType.Directory;
    public bool IsFile() => EntryType is PakEntryType.File;
    public bool IsInRoot() => EntryParent is PakFile;

    internal static PakEntry ReadPakEntry(BinaryReader reader, IPakEnumerable? parent) {
        return reader.PeekChar() switch {
            (byte) PakEntryType.Directory => (PakEntry) new PakDirectoryEntry(parent).ReadBinary(reader),
            (byte) PakEntryType.File => (PakEntry) new PakFileEntry(parent).ReadBinary(reader),
            _ => throw new Exception($"Unknown Pak entry type. {reader.PeekChar()}")
        };
    }
}