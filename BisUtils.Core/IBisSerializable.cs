namespace BisUtils.Core; 

public interface IBisSerializable {
    public IBisSerializable ReadBinary(BinaryReader reader);
    public void WriteBinary(BinaryWriter writer);
    
    public byte[] Binarize() {
        var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        WriteBinary(writer);
        return stream.ToArray();
    }

    public IBisSerializable Debinarize(byte[] data) => ReadBinary(new BinaryReader(new MemoryStream(data)));

    public static T FromBinary<T>(byte[] data) where T : IBisSerializable, new() => (T) new T().Debinarize(data);
}