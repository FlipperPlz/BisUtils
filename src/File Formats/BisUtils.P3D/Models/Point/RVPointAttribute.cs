namespace BisUtils.P3D.Models.Point;

using Core.Binarize;using Core.Binarize.Implementation;
using Core.IO;
using Data;
using FResults;
using Lod;
using Options;

public interface IRVPointAttrib<T> where T : struct
{
    public IRVLod Parent { get; set; }
    public List<T> Attributes { get; set; }
    T? this[int i] { get; set; }
}

public class RVPointAttrib<T> : IRVPointAttrib<T> where T : struct
{
    public IRVLod Parent { get; set; }
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

    public RVPointAttrib(IRVLod parentShape, List<T> attributes)
    {
        Parent = parentShape;
        Attributes = attributes;
    }


}
