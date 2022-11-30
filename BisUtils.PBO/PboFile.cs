using System.Security.Cryptography;
using System.Text;
using BisUtils.Core;
using BisUtils.Core.Compression;
using BisUtils.Core.Compression.Options;
using BisUtils.PBO.Builders;
using BisUtils.PBO.Entries;

namespace BisUtils.PBO;

public interface IPboFile : IBisSerializable {
    public byte[] GetEntryData(PboDataEntry dataEntry, bool decompress = true);
    public void OverwriteEntryData(PboDataEntry dataEntry, byte[] data, bool compressed = false);

    public void AddEntry(PboDataEntryDto dataEntryDto, bool syncStream = false);
    public void SyncToStream();
    public IEnumerable<BasePboEntry> GetPboEntries();
}

public class PboFile : IPboFile {
    public bool StreamIsSynced;
    
    public readonly Stream PboStream;
    public bool IsWritable => PboStream.CanWrite;
    
    private List<BasePboEntry> _pboEntries { get; set;}

    public ulong DataBlockStartOffset => _pboEntries.Aggregate<BasePboEntry, ulong>(0, (current, entry) => current + entry.CalculateMetaLength());
    public ulong DataBlockEndOffset => _pboEntries.Where(e => e is PboDataEntry).Cast<PboDataEntry>().Aggregate(DataBlockStartOffset, (current, entry) => current + entry.PackedSize);

    
    public PboFile(Stream pboStream) {
        PboStream = pboStream;
        _pboEntries = new List<BasePboEntry>();
        ReadBinary(new BinaryReader(pboStream, Encoding.UTF8, true));
        StreamIsSynced = true;
    }

    public byte[] GetEntryData(PboDataEntry dataEntry, bool decompress = true) {

        if (dataEntry is PboDataEntryDto dto) return dto.EntryData;

        using var reader = new BinaryReader(PboStream, Encoding.UTF8, true);
        reader.BaseStream.Seek((long)DataBlockStartOffset, SeekOrigin.Begin);
        reader.BaseStream.Position += (long) dataEntry.EntryDataStartOffset;
        
        return dataEntry.EntryMagic switch {
            PboEntryMagic.Compressed => decompress ? reader.ReadCompressedData<BisLZSSCompressionAlgorithms>(
                new BisLZSSDecompressionOptions() {
                    AlwaysDecompress = false, ExpectedSize = (int)dataEntry.OriginalSize, UseSignedChecksum = true
                }).ToArray() : reader.ReadBytes((int)dataEntry.PackedSize),
            PboEntryMagic.Decompressed => reader.ReadBytes((int)dataEntry.PackedSize),
            PboEntryMagic.Encrypted => throw new NotSupportedException("Encrypted PBOs are not supported by BisUtils."),
            PboEntryMagic.Version => throw new ArgumentOutOfRangeException(),
            PboEntryMagic.Undefined => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }


    public void AddEntry(PboDataEntryDto dataEntryDto, bool syncStream = false) {
        _pboEntries.Add(dataEntryDto);
        
        StreamIsSynced = false;
        if(syncStream) SyncToStream();
    }

    public void SyncToStream() {
        if(StreamIsSynced) return;
        throw new NotImplementedException();
    }

    public IEnumerable<BasePboEntry> GetPboEntries() => _pboEntries;
    
    //TODO: STILL NOT WORKING 
    public void OverwriteEntryData(PboDataEntry dataEntry, byte[] data, bool compressed = false) {
        
        if (dataEntry is PboDataEntryDto dto) {
            dto.EntryData = data;
            return;
        }
        if (!IsWritable) throw new Exception("The pbo file you're trying to write to is readonly.");
        
        var newDataSize = data.Length;
        if(compressed) {
            using var compressedDataStream = new MemoryStream();
            using (var compressionWriter = new BinaryWriter(compressedDataStream, Encoding.UTF8, true)) {
                compressionWriter.WriteCompressedData<BisLZSSCompressionAlgorithms>(data.ToArray(),
                    new BisLZSSCompressionOptions() { AlwaysCompress = false, WriteSignedChecksum = true });
            }
            compressedDataStream.Flush();
            data = compressedDataStream.ToArray();
        }
        var packedDataSize = data.Length;

        void WriteExtraData(byte[] fn_extraData) {
            using var dataAfterWriter = new BinaryWriter(PboStream, Encoding.UTF8, true);
            dataEntry.OriginalSize = (ulong) newDataSize;
            dataEntry.PackedSize = (ulong)packedDataSize;
            dataAfterWriter.BaseStream.Position = 0;
            foreach (var ent in _pboEntries) {
                if(ent == dataEntry) break;
                dataAfterWriter.BaseStream.Seek((long) ent.CalculateMetaLength(), SeekOrigin.Current);
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
            writer.Write(data.ToArray(), 0, data.Length);

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
            _pboEntries.Add(entry = BasePboEntry.ReadPboEntry(this, reader));
        } while (entry is not PboDummyEntry);

        foreach (var e in _pboEntries) {
            if(e is not PboDataEntry dataEntry) continue;
            dataEntry.ReinitializeOffsets();
        }

        reader.BaseStream.Seek((long) DataBlockEndOffset, SeekOrigin.Begin);
        
        //TODO: VALIDATE SHA AFTER DATA BLOCK 
        return this;
    }

    public void WriteBinary(BinaryWriter writer) {
        var dtos = _pboEntries.Where(e => e is PboDataEntryDto).Cast<PboDataEntryDto>().ToList();
        foreach (var entry in _pboEntries.Where(e => e is not PboDataEntryDto)) {
            if (entry is PboDummyEntry) foreach (var dtoEnt in dtos) dtoEnt.WriteBinary(writer);
            entry.WriteBinary(writer);
        }
        
        foreach (var entry in _pboEntries) {
            if(entry is not PboDataEntry dataEntry) continue;
            writer.Write(GetEntryData(dataEntry, false));
        }
        
        var checksum = CalculatePBOChecksum(writer.BaseStream);
            
        writer.Write((byte) 0x0);
        writer.Write(checksum);
    }

}