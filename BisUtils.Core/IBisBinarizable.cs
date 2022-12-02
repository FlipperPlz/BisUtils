namespace BisUtils.Core; 

public abstract class BisDebinarizationOptions : BisCommonBinarizationOptions {
    
}

public abstract class BisBinarizationOptions : BisCommonBinarizationOptions {
}

public abstract class BisCommonBinarizationOptions : BisOptions {
}

public interface IBisBinarizable<BO> where BO : BisCommonBinarizationOptions {
    public IBisBinarizable<BO> ReadBinary(BinaryReader reader, BO deserializationOptions);
    public void WriteBinary(BinaryWriter writer, BO serializationOptions);
    
    public virtual byte[] Binarize(BO binarizationOptions) {
        var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        WriteBinary(writer, binarizationOptions);
        return stream.ToArray();
    }

    public virtual IBisBinarizable<BO> Debinarize(byte[] data, BO debinarizationOptions) => 
        ReadBinary(new BinaryReader(new MemoryStream(data)), debinarizationOptions);

    public static T FromBinary<T>(byte[] data, BO debinarizationOptions) where T : IBisBinarizable<BO>, new() => (T) new T().Debinarize(data, debinarizationOptions);
}


public interface IBisBinarizable<DBO, BO> where DBO : BisDebinarizationOptions where BO : BisBinarizationOptions {
    public IBisBinarizable<DBO, BO> ReadBinary(BinaryReader reader, DBO debinarizationOptions);
    public void WriteBinary(BinaryWriter writer, BO binarizationOptions);
    
    public virtual byte[] Binarize(BO binarizationOptions) {
        var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        WriteBinary(writer, binarizationOptions);
        return stream.ToArray();
    }

    public virtual IBisBinarizable<DBO, BO> Debinarize(byte[] data, DBO debinarizationOptions) => 
        ReadBinary(new BinaryReader(new MemoryStream(data)), debinarizationOptions);

    public static T FromBinary<T>(byte[] data, DBO debinarizationOptions) where T : IBisBinarizable<DBO, BO>, new() => (T) new T().Debinarize(data, debinarizationOptions);
}

public interface IBisBinarizable {
    public IBisBinarizable ReadBinary(BinaryReader reader);
    public void WriteBinary(BinaryWriter writer);
    
    public virtual byte[] Binarize() {
        var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        WriteBinary(writer);
        return stream.ToArray();
    }

    public virtual IBisBinarizable Debinarize(byte[] data) => ReadBinary(new BinaryReader(new MemoryStream(data)));

    public static T FromBinary<T>(byte[] data) where T : IBisBinarizable, new() => (T) new T().Debinarize(data);
}

