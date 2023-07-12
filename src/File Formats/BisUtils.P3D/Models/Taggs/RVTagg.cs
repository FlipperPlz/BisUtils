namespace BisUtils.P3D.Models.Taggs;

using Core.Binarize.Implementation;
using Options;

public abstract class RVTagg : BinaryObject<RVShapeOptions>
{
    public string Name { get; set; }

    public int DataSize { get; set; }

    protected RVTagg(string name, int dataSize)
    {
        Name = name;
        DataSize = dataSize;
    }//TODO: Base Binarize
}
