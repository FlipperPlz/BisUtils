namespace BisUtils.Core.Family;

public interface IFamilyChild : IFamilyMember
{
    IFamilyParent? Parent { get; }
}
