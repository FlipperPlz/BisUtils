namespace BisUtils.Core.Family;

public interface IFamilyParent : IFamilyMember
{
    public IReadOnlyList<IFamilyMember> Children { get; set; }

}