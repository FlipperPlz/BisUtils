using System.Text;
using BisUtils.Core.Compression;
using BisUtils.Core.Compression.Options;
using BisUtils.Core.Serialization;
using BisUtils.PAK.Entries;
using BisUtils.PAK.Enums;
using BisUtils.PAK.Interfaces;

namespace BisUtils.PAK; 

public class PakFile : BisSynchronizable, IPakEnumerable, IBisBinarizable {
    public List<PakEntry> Children { get; } = new();
    public int FormSize { get; set; }
    public byte[] HeadData { get; set; } = null!;
    public int DataSize { get; set; }
    public int FileSize { get; set; }

    public int DataBlockStartOffset { get; set; }

    
    public PakFile(string path) : base(File.OpenRead(path)) =>
        ReadBinary(new BinaryReader(BaseStream, Encoding.UTF8));

    public override void SynchronizeStream(bool forceSynchronization = false) {
        base.SynchronizeStream(forceSynchronization);
        throw new NotSupportedException();
    }

    public byte[] GetEntryData(PakFileEntry fileEntry, bool decompress = true) {
        using var reader = new BinaryReader(BaseStream, Encoding.UTF8, true);
        reader.BaseStream.Seek(fileEntry.Offset, SeekOrigin.Begin);
        if (!decompress) goto ReturnDecompressed;
        if (fileEntry.CompressionType is PakCompressionType.ZLib) {
            try {
                var decompression = new BisZLibCompressionAlgorithms();
                using var decompressed =
                    reader.ReadCompressedData(decompression, new BisDecompressionOptions() {
                            ExpectedSize = fileEntry.OriginalSize
                        }
                    );
                return decompressed.ToArray();
            } catch (Exception e) {
                Console.WriteLine($"There was an error decompressing {fileEntry.GetPath()}");
                goto ReturnDecompressed;
            }
        }
        
        ReturnDecompressed:
        return reader.ReadBytes(fileEntry.PackedSize);
    }


    public IBisBinarizable ReadBinary(BinaryReader reader) {
        if (!reader.AssertMagic("FORM")) throw new Exception("Missing \"FORM\" Magic");
        FormSize = reader.ReadInt32BE();
        if (!reader.AssertMagic("PAC1")) throw new Exception("Missing \"PAC1\" Magic");
        if (!reader.AssertMagic("HEAD")) throw new Exception("Missing \"HEAD\" Magic");
        var headSize = reader.ReadInt32BE();
        HeadData = reader.ReadBytes(headSize);
        if (!reader.AssertMagic("DATA")) throw new Exception("Missing \"DATA\" Magic");
        DataSize = reader.ReadInt32BE();
        DataBlockStartOffset = (int) reader.BaseStream.Position;
        reader.BaseStream.Seek(DataSize, SeekOrigin.Current);
        if (!reader.AssertMagic("FILE")) throw new Exception("Missing \"FILE\" Magic");
        FileSize = reader.ReadInt32BE();
        var EOF = reader.BaseStream.Position + FileSize;
        reader.BaseStream.Seek(2, SeekOrigin.Current);
        var rootCount = reader.ReadInt32();
        do Children.Add(PakEntry.ReadPakEntry(reader, this));
        while (reader.BaseStream.Position < EOF);
        
        return this;
    }

    public void ExtractEntry(PakFileEntry fileEntry, string folder) {
        var outPath = Path.Combine(folder, fileEntry.GetPath());
        Directory.CreateDirectory(Directory.GetParent(outPath)!.FullName);
        File.WriteAllBytes(outPath, GetEntryData(fileEntry, true));
    }
    
    public void ExtractEntries(string folder) {
        foreach (var fileEntry in ((IPakEnumerable) this).GetFiles(true)) {
            ExtractEntry(fileEntry, folder);
        }
    }

    public void WriteBinary(BinaryWriter writer) {
        throw new NotSupportedException();
    }
}