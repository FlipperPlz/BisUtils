namespace BisUtils.Core; 

public abstract class BisDeserializationOptions : BisOptions {
    
}

public abstract class BisSerializationOptions : BisOptions {
}

public abstract class BisCommonSerializationOptions : BisOptions {
}

public interface IBisSerializable<SO> where SO : BisCommonSerializationOptions {
    public IBisSerializable ReadBinary(BinaryReader reader, SO deserializationOptions);
    public void WriteBinary(BinaryWriter writer, SO serializationOptions);
    
    public virtual byte[] Binarize(SO serializationOptions) {
        var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        WriteBinary(writer, serializationOptions);
        return stream.ToArray();
    }

    public virtual IBisSerializable Debinarize(byte[] data, SO deserializationOptions) => 
        ReadBinary(new BinaryReader(new MemoryStream(data)), deserializationOptions);

    public static T FromBinary<T>(byte[] data, SO deserializationOptions) where T : IBisSerializable<SO>, new() => (T) new T().Debinarize(data, deserializationOptions);
}


public interface IBisSerializable<DSO, SO> where DSO : BisDeserializationOptions where SO : BisSerializationOptions {
    public IBisSerializable ReadBinary(BinaryReader reader, DSO deserializationOptions);
    public void WriteBinary(BinaryWriter writer, SO serializationOptions);
    
    public virtual byte[] Binarize(SO serializationOptions) {
        var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        WriteBinary(writer, serializationOptions);
        return stream.ToArray();
    }

    public virtual IBisSerializable Debinarize(byte[] data, DSO deserializationOptions) => 
        ReadBinary(new BinaryReader(new MemoryStream(data)), deserializationOptions);

    public static T FromBinary<T>(byte[] data, DSO deserializationOptions) where T : IBisSerializable<DSO, SO>, new() => (T) new T().Debinarize(data, deserializationOptions);
}

public interface IBisSerializable {
    public IBisSerializable ReadBinary(BinaryReader reader);
    public void WriteBinary(BinaryWriter writer);
    
    public virtual byte[] Binarize() {
        var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        WriteBinary(writer);
        return stream.ToArray();
    }

    public virtual IBisSerializable Debinarize(byte[] data) => ReadBinary(new BinaryReader(new MemoryStream(data)));

    public static T FromBinary<T>(byte[] data) where T : IBisSerializable, new() => (T) new T().Debinarize(data);
}

