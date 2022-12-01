using System.Text;
using BisUtils.Core;
using BisUtils.Core.Compression;
using BisUtils.Core.Compression.Options;
using BisUtils.PBO.Builders;
using BisUtils.PBO.Entries;
using BisUtils.PBO.Extensions;

namespace BisUtils.PBO;

public interface IPboFile : IBisSerializable<PboDeserializationOptions, PboSerializationOptions> {
    public byte[] GetEntryData(PboDataEntry dataEntry, bool decompress = true);
    public void OverwriteEntryData(PboDataEntry dataEntry, byte[] data, bool compressed = false);

    public void AddEntry(PboDataEntryDto dataEntryDto, bool syncStream = false);
    public void SyncToStream();
    public void DeSyncStream();
    public IEnumerable<BasePboEntry> GetPboEntries();
    public IEnumerable<PboVersionEntry>? GetVersionEntries();
    public PboVersionEntry? GetVersionEntry();

}

public class PboFile : IPboFile {
    public bool StreamIsSynced;
    
    public readonly Stream PboStream;
    public bool IsWritable => PboStream.CanWrite;
    
    private List<BasePboEntry> _pboEntries { get; set;}

    public ulong DataBlockStartOffset => _pboEntries.Aggregate<BasePboEntry, ulong>(0, (current, entry) => current + entry.CalculateMetaLength());
    public ulong DataBlockEndOffset => _pboEntries.Where(e => e is PboDataEntry).Cast<PboDataEntry>().Aggregate(DataBlockStartOffset, (current, entry) => current + entry.PackedSize);

    
    public PboFile(Stream pboStream, PboFileOption option = PboFileOption.Read) {
        PboStream = pboStream;
        _pboEntries = new List<BasePboEntry>();

        switch (option) {
            case PboFileOption.Read: {
                ReadBinary(new BinaryReader(pboStream, Encoding.UTF8, true));
                break;
            }
            case PboFileOption.Create: {
                _pboEntries.Add(new PboVersionEntry(this));
                _pboEntries.Add(new PboDummyEntry(this));
                WriteBinary(new BinaryWriter(pboStream, Encoding.UTF8, true));
                break;
            }
            default: throw new ArgumentOutOfRangeException(option.ToString());
        }
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
            _ => throw new ArgumentOutOfRangeException(dataEntry.EntryMagic.ToString())
        };
    }


    public void AddEntry(PboDataEntryDto dataEntryDto, bool syncPbo = false) {
        StreamIsSynced = false;
        //TODO: BROKEN, ADDING A DTO FUCKS UP THE OFFSETS BREAKING FILES
        _pboEntries.Add(dataEntryDto);
        
        if(syncPbo) SyncToStream();
    }

    public void SyncToStream() {
        if(!IsWritable) throw new Exception("Cannot sync a readonly stream. Try opening the PBO with write access or writing to a new file.");

        if(StreamIsSynced) return;

        using (var newStream = new MemoryStream()) {
            using (var newPboWriter = new BinaryWriter(newStream, Encoding.UTF8, true)) {
                WriteBinary(newPboWriter);
                
                newPboWriter.Flush();
            }
            PboStream.SetLength(0);

            PboStream.SetLength(newStream.Length);
            newStream.WriteTo(PboStream);
        }

        PboStream.Seek(0, SeekOrigin.Begin);
        _pboEntries = new List<BasePboEntry>();
        ReadBinary(new BinaryReader(PboStream, Encoding.UTF8, true));

        StreamIsSynced = true;
    }

    public void DeSyncStream() => StreamIsSynced = false;
    
    public IEnumerable<BasePboEntry> GetPboEntries() => _pboEntries;

    // ReSharper disable once ReturnTypeCanBeNotNullable RedundantAssignment
    public IEnumerable<PboVersionEntry>? GetVersionEntries() {
        var versionEntries = _pboEntries.Where(b => b is PboVersionEntry).Cast<PboVersionEntry>();
        var pboVersionEntries = versionEntries as PboVersionEntry[] ?? versionEntries.ToArray();
        if (!pboVersionEntries.Any()) versionEntries = null;
        return pboVersionEntries;
    }

    public PboVersionEntry? GetVersionEntry() {
        var versionEntries = GetVersionEntries();
        if (versionEntries is null) return null;

        var versionEntriesArr = versionEntries.ToArray();

        return versionEntriesArr.First();
    }

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
            dataAfterWriter.WritePboChecksum();
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

    public IBisSerializable<PboDeserializationOptions, PboSerializationOptions> ReadBinary(BinaryReader reader, PboDeserializationOptions? options = null) {
        options ??= PboDeserializationOptions.DefaultOptions;

        
        BasePboEntry entry;

        if (options.StrictVersionEntry) {
            _pboEntries.Add(entry = BasePboEntry.ReadPboEntry(this, reader));
            if (entry is not PboVersionEntry) throw new Exception($"Expected a starting version entry, instead got {entry.GetType().Name}");
        }
        
        do {
            _pboEntries.Add(entry = BasePboEntry.ReadPboEntry(this, reader));
            if (options.StrictVersionEntry && entry is PboVersionEntry)
                throw new Exception("Only one version entry is allowed in strict mode.");

        } while (entry is not PboDummyEntry);

        foreach (var e in _pboEntries) {
            if(e is not PboDataEntry dataEntry) continue;
            dataEntry.ReinitializeOffsets();
        }

        reader.BaseStream.Seek((long) DataBlockEndOffset, SeekOrigin.Begin);

        if (options.RequireVersionEntry && GetVersionEntry() is null) throw new Exception("No version entry was found while reading pbo.");

        if (!options.VerifyChecksum) return this;
        
        //if (!reader.VerifyPboChecksum()) throw new Exception("The PBO checksum does not match the one calculated.");
        return this;
    }

    public void WriteBinary(BinaryWriter writer, PboSerializationOptions? options = null) {
        options ??= PboSerializationOptions.DefaultOptions;

        if (options.RequireVersionEntry) {
            if (!_pboEntries.Where(v => v is PboVersionEntry).ToArray().Any())
                throw new Exception("Cannot write PBO without a version entry.");
        }
        
        if (options.RequireDummyEntry) {
            if (!_pboEntries.Where(v => v is PboDummyEntry).ToArray().Any())
                throw new Exception("Cannot write PBO without a dummy data entry.");
        }

        if (options.StrictVersionEntry) {
            if (_pboEntries.First() is not PboVersionEntry)
                throw new Exception("In strict mode there must be a single version entry at the beginning of the pbo.");
            if (_pboEntries.Where(v => v is PboDummyEntry).ToArray().Length > 1)
                throw new Exception("In strict mode there can only be a single version entry.");
        }

        var dtos = _pboEntries.Where(e => e is PboDataEntryDto).Cast<PboDataEntryDto>();
        
        
        
        foreach (var entry in _pboEntries) {
            if(entry is PboDataEntryDto) continue;
            if (entry is PboDummyEntry) {
                foreach (var dtoEnt in dtos) dtoEnt.WriteBinary(writer);
            }
                
            entry.WriteBinary(writer);
        }
        
        foreach (var entry in _pboEntries) {
            if(entry is not PboDataEntry dataEntry) continue;
            if(entry is PboDataEntryDto ) continue; 
            writer.Write(GetEntryData(dataEntry, false));
        }

        writer.Seek(-21, SeekOrigin.Current);

        
        foreach (var entry in dtos) {
            entry.WriteEntryData(writer);
            entry.RewriteMetadata(writer);
        }
        
        writer.WritePboChecksum();
        writer.Flush();
        writer.BaseStream.Flush();
    }

}