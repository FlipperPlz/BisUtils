using System.Text;
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
        try {
            do 
                Children.Add(PakEntry.ReadPakEntry(reader, this));
            while (reader.BaseStream.Position < EOF);
        } catch (Exception e) {
            Console.WriteLine($"CRASHED AT {reader.ReadInt32()}");
            //TODO FIX
        }
        
        
        return this;
    }

    public void WriteBinary(BinaryWriter writer) {
        throw new NotSupportedException();
    }
}