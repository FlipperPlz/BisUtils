using System.Text;

namespace BisUtils.Core; 

public abstract class BisDeserializationOptions : BisCommonSerializationOptions {
    
}

public abstract class BisSerializationOptions : BisCommonSerializationOptions {
    public int Indentation { get; set; } = 0;
}

public abstract class BisCommonSerializationOptions : BisOptions {
}

public interface IBisSerializable<DSO, SO> where DSO : BisDeserializationOptions where SO : BisSerializationOptions {
    public IBisBinarizable FromString(StringBuilder builder, DSO deserializationOptions);
    public void WriteBinary(StringBuilder builder, SO serializationOptions);

    public virtual string ToString(SO options) {
        var builder = new StringBuilder();
        WriteBinary(builder, options);
        return builder.ToString();
    }
}

public interface IBisSerializable<CSO> where CSO : BisCommonSerializationOptions {
    public IBisBinarizable FromString(StringBuilder builder, CSO deserializationOptions);
    public void WriteBinary(StringBuilder builder, CSO serializationOptions);

    public virtual string ToString(CSO options) {
        var builder = new StringBuilder();
        WriteBinary(builder, options);
        return builder.ToString();
    }
}

public interface IBisSerializable {
    public IBisBinarizable FromString(StringBuilder builder);
    public void WriteBinary(StringBuilder builder);

    public virtual string ToString() {
        var builder = new StringBuilder();
        WriteBinary(builder);
        return builder.ToString();
    }
}