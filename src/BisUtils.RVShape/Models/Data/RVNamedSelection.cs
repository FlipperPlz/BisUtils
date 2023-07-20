namespace BisUtils.RVShape.Models.Data;

using BisUtils.RVShape.Models.Lod;

public interface IRVNamedSelection : IRVSelection
{
    public string Name { get; set; }
}

public class RVNamedSelection : RVSelection, IRVNamedSelection
{
    public string Name { get; set; }

    public RVNamedSelection(IRVLod lod, string name) : base(lod) => Name = name;
}
