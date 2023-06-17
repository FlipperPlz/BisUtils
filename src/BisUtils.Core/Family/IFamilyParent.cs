namespace BisUtils.Core.Family;

public interface IFamilyParent : IFamilyChild
{
    IEnumerable<IFamilyMember> Children { get; }
}
