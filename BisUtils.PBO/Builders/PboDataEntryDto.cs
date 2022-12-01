using System.Text;
using BisUtils.Core.Compression;
using BisUtils.Core.Compression.Options;
using BisUtils.PBO.Entries;

namespace BisUtils.PBO.Builders; 

public class PboDataEntryDto : PboDataEntry {
    private Stream _entryStream { get; set; }
    public ulong? EntryMetaStartOffset;
    
    public override byte[] EntryData {
        get {
            var startPos = _entryStream.Position;
            
            _entryStream.Seek(0, SeekOrigin.Begin);
            using var memoryStream = new MemoryStream();
            _entryStream.CopyTo(memoryStream);
            _entryStream.Seek(startPos, SeekOrigin.Begin);

            return memoryStream.ToArray();
        }
        set => ChangeData(value, true);
    }

    public void RewriteMetadata(BinaryWriter writer) {
        if (EntryMetaStartOffset is 0 or null)
            throw new Exception("In order to rewrite the entry meta, It has to have been re/written previously.");
        var startPos = writer.BaseStream.Position;
        writer.Seek((int) EntryMetaStartOffset, SeekOrigin.Begin);
        byte[]? entryMeta;
            
        using (var metaStream = new MemoryStream()) {
            WriteBinary(new BinaryWriter(metaStream, Encoding.UTF8, true));
            entryMeta = metaStream.ToArray();
        }

        if (entryMeta is null) throw new Exception($"Failed to rewrite entry meta for dto {EntryName}");
            
        writer.Write(entryMeta, 0, entryMeta.Length);
        writer.Seek((int) startPos, SeekOrigin.Begin);
    }


    public void ChangeData(byte[] newData, bool replaceStream = false) {
        switch (replaceStream) {
            case true: {
                _entryStream = new MemoryStream(newData);
                _entryStream.Flush();
                return;
            }
            case false: {
                _entryStream.SetLength(newData.LongLength);
                using (var writer = new BinaryWriter(_entryStream, Encoding.UTF8, true)) {
                    writer.BaseStream.Seek(0, SeekOrigin.Begin);
                   writer.Write(newData);
                   
                   writer.Flush();
                }
                
                _entryStream.Flush();
                return;
            }
        }
    }

    public override void WriteBinary(BinaryWriter writer) {
        EntryMetaStartOffset = (ulong) writer.BaseStream.Position;
        base.WriteBinary(writer);
    }

    public PboDataEntryDto(PboFile entryParent, Stream entryData, ulong? timestamp = null, bool compress = false) : base(entryParent) {
         _entryStream = entryData;
         TimeStamp = timestamp ?? (ulong) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
         OriginalSize = (ulong) _entryStream.Length;
         EntryMagic = compress ? PboEntryMagic.Compressed : PboEntryMagic.Decompressed;
    }

    public void WriteEntryData(BinaryWriter writer) {
        var ogPos = writer.BaseStream.Position;
        switch (EntryMagic) {
            case PboEntryMagic.Compressed: {
                writer.WriteCompressedData<BisLZSSCompressionAlgorithms>(EntryData,
                    new BisLZSSCompressionOptions() { AlwaysCompress = false, WriteSignedChecksum = true });
                break;
            }
            case PboEntryMagic.Decompressed: {
                writer.Write(EntryData);
                break;
            }
            case PboEntryMagic.Encrypted: throw new NotSupportedException();
            default: throw new ArgumentOutOfRangeException(EntryMagic.ToString());
        }
        PackedSize = (ulong) (writer.BaseStream.Position - ogPos);
    }
    
}