namespace BisUtils.Core.Family;

public interface IFamilyParent : IFamilyChild
{
    public IEnumerable<IFamilyMember> Children { get; }

}