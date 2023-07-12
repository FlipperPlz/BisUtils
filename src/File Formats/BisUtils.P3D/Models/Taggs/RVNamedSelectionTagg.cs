namespace BisUtils.P3D.Models.Taggs;

using Core.IO;
using FResults;
using Options;

public class RVNamedSelectionTagg : RVTagg
{
    public RVNamedSelectionTagg(string name, int dataSize)
        : base(name, dataSize)
    {

    }

    public override Result Binarize(BisBinaryWriter writer, RVShapeOptions options)
    {
        writer.WriteAsciiZ(Name, options);
        writer.Write(DataSize);
        return Result.Ok();
    }

    public override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        reader.ReadAsciiZ(out var name, options);
        Name = name;
        DataSize = reader.ReadInt32();
        return Result.Ok();
    }

}
