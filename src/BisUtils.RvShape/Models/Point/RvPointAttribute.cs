namespace BisUtils.RvShape.Models.Point;

using Lod;

public interface IRvPointAttrib<T> where T : struct
{
    public IRvLod Parent { get; set; }
    public List<T> Attributes { get; set; }
    T? this[int i] { get; set; }
}

public class RvPointAttrib<T> : IRvPointAttrib<T> where T : struct
{
    public IRvLod Parent { get; set; }
    public List<T> Attributes { get; set; }

    public T? this[int i]
    {
        get => Attributes.Count < i ? null : Attributes[i];
        set
        {
            if (value is { } notnull)
            {
                Attributes[i] = notnull;
            }
        }
    }

    public RvPointAttrib(IRvLod parentShape, List<T> attributes)
    {
        Parent = parentShape;
        Attributes = attributes;
    }


}
