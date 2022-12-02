using System.Text;
using BisUtils.Core;
using BisUtils.Core.Compression;
using BisUtils.Core.Compression.Options;
using BisUtils.PBO.Builders;
using BisUtils.PBO.Entries;
using BisUtils.PBO.Extensions;

namespace BisUtils.PBO;

public interface IPboFile : IBisSerializable<PboDeserializationOptions, PboSerializationOptions> {
    byte[] GetEntryData(PboDataEntry dataEntry, bool decompress = true);
    void OverwriteEntryData(PboDataEntry dataEntry, byte[] data, bool compressed = false, bool syncToStream = true);

    void AddEntry(PboDataEntryDto dataEntryDto, bool syncStream = false);
    void DeleteEntry(PboDataEntry dataEntry, bool syncStream = false);

    void SyncToStream();
    void DeSyncStream();

    IEnumerable<BasePboEntry> GetPboEntries();
    IEnumerable<PboVersionEntry>? GetVersionEntries();
    PboVersionEntry? GetVersionEntry();

}

public class PboFile : IPboFile {
    private bool _streamIsSynced;
    
    public readonly Stream PboStream;
    public bool IsWritable => PboStream.CanWrite;
    
    private List<BasePboEntry> _pboEntries { get; set;}
    public ulong DataBlockStartOffset => _pboEntries.Where(entry => entry is not PboDataEntryDto).Aggregate<BasePboEntry, ulong>(0, (current, entry) => current + entry.CalculateMetaLength());
    public ulong DataBlockEndOffset => _pboEntries.Where(e => e is PboDataEntry and not PboDataEntryDto).Cast<PboDataEntry>().Aggregate(DataBlockStartOffset, (current, entry) => current + entry.PackedSize);

    
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
        _streamIsSynced = true;
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
        _streamIsSynced = false;
        _pboEntries.Add(dataEntryDto);
        
        if(syncPbo) SyncToStream();
    }

    public void SyncToStream() {
        if(!IsWritable) throw new Exception("Cannot sync a readonly stream. Try opening the PBO with write access or writing to a new file.");

        if(_streamIsSynced) return;

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
        _streamIsSynced = true;
    }

    public void DeSyncStream() => _streamIsSynced = false;
    
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

    public void DeleteEntry(PboDataEntry dataEntry, bool syncStream = false) {
        if (dataEntry.EntryParent != this) throw new Exception("Cannot delete an entry outside of the pbo.");

        if(!dataEntry.IsQueuedForDeletion()) dataEntry.QueueDeletion();
        if(syncStream) SyncToStream();
    }


    private ulong GetMetadataOffset(PboDataEntry dataEntry) {
        if (dataEntry is PboDataEntryDto)
            throw new Exception("dtos's are not stream-synced and therefore have no offsets.");
        if (dataEntry.EntryParent != this) throw new Exception("Cannot calculate offset for an entry outside of this pbo instance.");

        ulong offset = 0;
        foreach (var ent in _pboEntries) {
            if (ent is PboDataEntryDto) continue;
            if (ent == dataEntry) break;
            offset += ent.CalculateMetaLength();
        }

        return offset;
    }
    
    //TODO: STILL NOT WORKING 
    public void OverwriteEntryData(PboDataEntry dataEntry, byte[] data, bool compressed = false, bool syncStream = false) {
        if (dataEntry is PboDataEntryDto dto) {
            dto.EntryData = data;
            return;
        }
        if (!IsWritable && syncStream) throw new Exception("The pbo file you're trying to write to is readonly.");
        
        AddEntry(new PboDataEntryDto(this, new MemoryStream(data), dataEntry.TimeStamp, compressed), syncStream);
        DeleteEntry(dataEntry, syncStream);
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

        var dtos = _pboEntries.Where(e => e is PboDataEntryDto).Cast<PboDataEntryDto>().ToList();
        
        
        
        foreach (var entry in _pboEntries) {
            if (entry is PboDataEntry pboDataEntry && options.UseCommonTimeStamp is { } timeStamp) 
                pboDataEntry.TimeStamp = timeStamp;

            switch (entry) {
                case PboDataEntryDto: continue;
                case PboDataEntry dataEntry when dataEntry.IsQueuedForDeletion(): continue;
                case PboDummyEntry: {
                    foreach (var dtoEnt in dtos) {
                        if(dtoEnt.IsQueuedForDeletion()) continue;
                        dtoEnt.WriteBinary(writer);
                    }
                    break;
                }
            }

            entry.WriteBinary(writer);
        }
        
        foreach (var entry in _pboEntries) {
            if(entry is not PboDataEntry dataEntry) continue;
            if(dataEntry.IsQueuedForDeletion()) continue;
            if(entry is PboDataEntryDto ) continue; 
            writer.Write(GetEntryData(dataEntry, false));
        }

        foreach (var entry in dtos) {
            if(entry.IsQueuedForDeletion()) continue;
            entry.WriteEntryData(writer);
            entry.RewriteMetadata(writer);
        }

        if (options.WriteDataOffsets) {
            var startPos = writer.BaseStream.Position;
            foreach (var entry in _pboEntries) {
                if(entry is not PboDataEntry dataEntry) continue;
                var dataOffset = DataBlockStartOffset + dataEntry.EntryDataStartOffset;
                writer.Seek((int) GetMetadataOffset(dataEntry), SeekOrigin.Begin);
                writer.Seek(Encoding.UTF8.GetBytes(dataEntry.EntryName).Length + 9, SeekOrigin.Current);
                writer.Write(BitConverter.GetBytes((int) dataOffset), 0, 4);
            }

            writer.Seek((int)startPos, SeekOrigin.Begin);
        }
        
        writer.WritePboChecksum();
        writer.Flush();
        writer.BaseStream.Flush();
    }

}