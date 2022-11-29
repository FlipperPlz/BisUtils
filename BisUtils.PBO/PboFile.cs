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

    public MemoryStream ReadEntryData(PboDataEntry dataEntry) {
        using var reader = new BinaryReader(PboStream, Encoding.UTF8, true);
        reader.BaseStream.Seek((long)DataBlockStartOffset, SeekOrigin.Begin);
        reader.BaseStream.Position += (long) dataEntry.EntryDataStartOffset;

        return dataEntry.EntryMagic switch {
            PboEntryMagic.Compressed => reader.ReadCompressedData<BisLZSSCompressionAlgorithms>(
                new BisLZSSDecompressionOptions() {
                    AlwaysDecompress = false, ExpectedSize = (int)dataEntry.OriginalSize, UseSignedChecksum = true
                }),
            PboEntryMagic.Decompressed => new MemoryStream(reader.ReadBytes((int)dataEntry.DataLength)),
            PboEntryMagic.Encrypted => throw new NotSupportedException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void OverwriteEntryData(PboDataEntry dataEntry, MemoryStream data, bool compressed = false) {
        //TODO: UNTESTED NON EFFICIENT CODE I PULLED OUT OF MY ASSHOLE.
        //TODO: Hopefully it reflects my idea of overwriting data. 
        if (!IsWritable) throw new Exception("The pbo file you're trying to write to is readonly.");
        
        var newDataSize = data.Length;
        if(compressed) {
            var compressedDataStream = new MemoryStream();
            using (var compressionWriter = new BinaryWriter(compressedDataStream, Encoding.UTF8, true)) {
                compressionWriter.WriteCompressedData<BisLZSSCompressionAlgorithms>(data.ToArray(),
                    new BisLZSSCompressionOptions() {AlwaysCompress = false, WriteSignedChecksum = true});
            }
            data.Flush();
            data.Dispose();
            data = compressedDataStream;
        }
        var packedDataSize = data.Length;
        var dataAfter = new MemoryStream();
        

        using (var writer = new BinaryWriter(PboStream, Encoding.UTF8, true)) {
            writer.BaseStream.Seek((long) DataBlockStartOffset, SeekOrigin.Begin);
            writer.BaseStream.Seek((long) dataEntry.EntryDataStartOffset, SeekOrigin.Current);
            
            using (var dataAfterWriter = new BinaryWriter(dataAfter, Encoding.UTF8, true)) {
                using var dataAfterReader = new BinaryReader(PboStream, Encoding.UTF8, true);
                dataAfterReader.BaseStream.Seek((long) DataBlockStartOffset, SeekOrigin.Begin);
                dataAfterReader.BaseStream.Seek((long) dataEntry.EntryDataStopOffset, SeekOrigin.Current);
                dataAfterWriter.Write(dataAfterReader.ReadBytes((int) (dataAfterReader.BaseStream.Position - dataAfterReader.BaseStream.Length)));
            }
        
            writer.BaseStream.SetLength(writer.BaseStream.Position);
        
            dataAfter.Flush();
            dataAfter.Close();
            writer.Flush();
            writer.Close();
        }

        using (var writer = new BinaryWriter(PboStream, Encoding.UTF8, true)) {
            writer.BaseStream.Seek(0, SeekOrigin.End);
            writer.Write(data.ToArray());
            writer.Write(dataAfter.ToArray());
            writer.BaseStream.Seek(0, SeekOrigin.Begin);

            foreach (var ent in PboEntries) {
                if (ent == dataEntry) break;
                writer.BaseStream.Seek((long) ent.CalculateMetaLength(), SeekOrigin.Current);
            }
            
            writer.BaseStream.Seek(Encoding.UTF8.GetBytes(dataEntry.EntryName).Length, SeekOrigin.Current);
            writer.BaseStream.Seek(1, SeekOrigin.Current);
            writer.Write( BitConverter.GetBytes(compressed ? (int) PboEntryMagic.Compressed : (int) PboEntryMagic.Decompressed), 0, 4);
            writer.Write(BitConverter.GetBytes(newDataSize), 0, 4); //TODO: check to see if this advances stream by 5
            writer.BaseStream.Seek(8, SeekOrigin.Current);
            writer.Write(BitConverter.GetBytes(packedDataSize), 0, 4); //TODO: check to see if this advances stream by 5
            writer.BaseStream.SetLength(writer.BaseStream.Position);
            
            writer.Flush();
            writer.Close();
        }


        using (var writer = new BinaryWriter(PboStream, Encoding.UTF8, true)) {
            var checksum = CalculatePBOChecksum(writer.BaseStream);
            writer.Write((byte) 0x0);
            writer.Write(checksum);
            
            writer.Flush();
            writer.Close();
        }
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
        throw new NotImplementedException(); //TODO: PBO WRITE
    }

}