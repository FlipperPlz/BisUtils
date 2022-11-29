using System.Security.Cryptography;
using System.Text;
using BisUtils.Core;
using BisUtils.Core.Compression;
using BisUtils.Core.Compression.Options;
using BisUtils.PBO.Entries;

namespace BisUtils.PBO;

public interface IPboFile : IBisSerializable {
    
}

public class PboFile : IPboFile {
    public readonly Stream PboStream;
    public bool IsWritable => PboStream.CanWrite;
    public List<BasePboEntry> PboEntries { get; set; }

    public ulong DataBlockStartOffset => PboEntries.Aggregate<BasePboEntry?, ulong>(0, (current, entry) => current + entry switch {
        //Notice: C# for some reason requires this cast to invoke the "new" version of PboVersionEntry::CalculateMetaLength()
        PboVersionEntry versionEntry => versionEntry.CalculateMetaLength(),
        _ => entry!.CalculateMetaLength()
    });

    public ulong DataBlockEndOffset => PboEntries.Aggregate(DataBlockStartOffset, (current, entry) => current + entry.DataLength);


    public PboFile(Stream pboStream) {
        PboStream = pboStream;
        PboEntries = new List<BasePboEntry>();
        ReadBinary(new BinaryReader(pboStream, Encoding.UTF8, true));
    }

    public MemoryStream ReadEntryData(PboDataEntry dataEntry, bool decompress = true) {
        using var reader = new BinaryReader(PboStream, Encoding.UTF8, true);
        reader.BaseStream.Seek((long)DataBlockStartOffset, SeekOrigin.Begin);
        reader.BaseStream.Position += (long) dataEntry.EntryDataStartOffset;
        
        return dataEntry.EntryMagic switch {
            PboEntryMagic.Compressed => decompress ? reader.ReadCompressedData<BisLZSSCompressionAlgorithms>(
                new BisLZSSDecompressionOptions() {
                    AlwaysDecompress = false, ExpectedSize = (int)dataEntry.OriginalSize, UseSignedChecksum = true
                }) : new MemoryStream(reader.ReadBytes((int)dataEntry.DataLength)),
            PboEntryMagic.Decompressed => new MemoryStream(reader.ReadBytes((int)dataEntry.DataLength)),
            PboEntryMagic.Encrypted => throw new NotSupportedException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void OverwriteEntryData(PboDataEntry dataEntry, MemoryStream data, bool compressed = false) {
        //TODO: STILL NOT WORKING 
        if (!IsWritable) throw new Exception("The pbo file you're trying to write to is readonly.");
        
        var newDataSize = data.Length;
        if(compressed) {
            var compressedDataStream = new MemoryStream();
            using (var compressionWriter = new BinaryWriter(compressedDataStream, Encoding.UTF8, true)) {
                compressionWriter.WriteCompressedData<BisLZSSCompressionAlgorithms>(data.ToArray(),
                    new BisLZSSCompressionOptions() { AlwaysCompress = false, WriteSignedChecksum = true });
            }

            data = compressedDataStream;
        }
        var packedDataSize = data.Length;

        void WriteExtraData(byte[] fn_extraData) {
            using var dataAfterWriter = new BinaryWriter(PboStream, Encoding.UTF8, true);
            dataEntry.OriginalSize = (ulong) newDataSize;
            dataEntry.PackedSize = (ulong)packedDataSize;
            dataAfterWriter.BaseStream.Position = 0;
            foreach (var ent in PboEntries) {
                if(ent == dataEntry) break;
                switch (ent) {
                    case PboVersionEntry versionEntry:
                        dataAfterWriter.BaseStream.Seek((long) versionEntry.CalculateMetaLength(), SeekOrigin.Current);
                        break;
                    default:
                        dataAfterWriter.BaseStream.Seek((long) ent.CalculateMetaLength(), SeekOrigin.Current);
                        break;
                }
            }
            dataAfterWriter.BaseStream.Seek((long) Encoding.UTF8.GetBytes(dataEntry.EntryName).Length + 5, SeekOrigin.Current);
            dataAfterWriter.Write(BitConverter.GetBytes((int) dataEntry.OriginalSize), 0, 4);
            dataAfterWriter.BaseStream.Seek(8, SeekOrigin.Current);
            dataAfterWriter.Write(BitConverter.GetBytes((int) dataEntry.PackedSize), 0, 4);
                        
            dataAfterWriter.BaseStream.Position = dataAfterWriter.BaseStream.Length;
            dataAfterWriter.Write(fn_extraData);
            var checksum = CalculatePBOChecksum(dataAfterWriter.BaseStream);
            dataAfterWriter.Write((byte) 0x0);
            dataAfterWriter.Write(checksum);
            dataAfterWriter.Flush();
        }

        using var writer = new BinaryWriter(PboStream, Encoding.UTF8, true);
        writer.BaseStream.Seek((long) DataBlockStartOffset, SeekOrigin.Begin);
        writer.BaseStream.Position += (long) dataEntry.EntryDataStartOffset;

        //Check to see if we already have enough space to overwrite
        if (data.Length <= (long) dataEntry.PackedSize) {
            writer.Write(data.ToArray(), 0, (int) data.Length);

            var leftoverBytesCount = dataEntry.PackedSize - (ulong) data.Length;
                
            //Remove Leftover Data
            if (leftoverBytesCount > 0) {
                byte[] extraData;
                using (var dataAfterReader = new BinaryReader(PboStream, Encoding.UTF8, true)) {
                    dataAfterReader.BaseStream.Seek(writer.BaseStream.Position + (long) leftoverBytesCount, SeekOrigin.Begin);
                    extraData = dataAfterReader.ReadBytes((int)(DataBlockEndOffset - (ulong)dataAfterReader.BaseStream.Position));
                }
                writer.BaseStream.SetLength(writer.BaseStream.Position);
                writer.Flush();
                WriteExtraData(extraData);
                return;
            }
        }
        //We dont have enough space so lets use what we have and make the rest as we go
        writer.Write(data.ToArray()[..(int) dataEntry.PackedSize], 0, (int) dataEntry.PackedSize);
        writer.Write(data.ToArray()[(int) dataEntry.PackedSize..]);

        byte[] extraEntryData;
        using (var dataAfterReader = new BinaryReader(PboStream, Encoding.UTF8, true)) {
            dataAfterReader.BaseStream.Seek(writer.BaseStream.Position, SeekOrigin.Begin);
            extraEntryData = dataAfterReader.ReadBytes(
                (int)(DataBlockEndOffset - (ulong) dataAfterReader.BaseStream.Position));
        }
            
        WriteExtraData(extraEntryData);
            
        writer.BaseStream.SetLength(writer.BaseStream.Position);
        writer.Flush();
    } 

    private static byte[] CalculatePBOChecksum(Stream stream) {
        var oldPos = stream.Position;

        stream.Position = 0;
#pragma warning disable 
        var hash = new SHA1Managed().ComputeHash(stream);
#pragma warning restore 

        stream.Position = oldPos;

        return hash;
    }

    public IBisSerializable ReadBinary(BinaryReader reader) {
        
        BasePboEntry entry;
        do {
            PboEntries.Add(entry = BasePboEntry.ReadPboEntry(this, reader));
        } while (entry is not PboDummyEntry);

        foreach (var e in PboEntries) {
            if(e is not PboDataEntry dataEntry) continue;
            dataEntry.ReinitializeOffsets();
        }

        reader.BaseStream.Seek((long) DataBlockEndOffset, SeekOrigin.Begin);
        
        //TODO: VALIDATE SHA AFTER DATA BLOCK 
        return this;
    }

    public void WriteBinary(BinaryWriter writer) {
        foreach (var entry in PboEntries) entry.WriteBinary(writer);
        foreach (var entry in PboEntries) {
            if(entry is not PboDataEntry dataEntry) continue;
            
            using (var entryData = ReadEntryData(dataEntry, false)) writer.Write(entryData.ToArray());
            
            var checksum = CalculatePBOChecksum(writer.BaseStream);
            
            writer.Write((byte) 0x0);
            writer.Write(checksum);
        }
    }

}