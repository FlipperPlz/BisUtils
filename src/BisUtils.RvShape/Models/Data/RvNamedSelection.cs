namespace BisUtils.RvShape.Models.Data;

using Lod;

public interface IRvNamedSelection : IRvSelection
{
    public string Name { get; set; }
}

public class RvNamedSelection : RvSelection, IRvNamedSelection
{
    public string Name { get; set; }

    public RvNamedSelection(IRvLod lod, string name) : base(lod) => Name = name;
}
