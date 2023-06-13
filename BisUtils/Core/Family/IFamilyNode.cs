using System.Collections.ObjectModel;

namespace BisUtils.Core.Family;

public interface IFamilyNode : IFamilyParent
{
    IFamilyNode? IFamilyMember.Node
    {
        get => this;
        set => throw new NotImplementedException();
    }
}