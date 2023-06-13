namespace BisUtils.Core.Family;

public interface IFamilyChild : IFamilyMember
{
    public IFamilyParent? Parent { get; }
    
}