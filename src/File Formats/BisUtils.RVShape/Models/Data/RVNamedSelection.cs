namespace BisUtils.P3D.Models.Data;

using Lod;

public interface IRVNamedSelection : IRVSelection
{
    public string Name { get; set; }
}

public class RVNamedSelection : RVSelection, IRVNamedSelection
{
    public string Name { get; set; }

    public RVNamedSelection(IRVLod lod, string name) : base(lod) => Name = name;
}
