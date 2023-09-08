namespace BisUtils.RvConfig.Models.Stubs.Holders;

public interface IParamLiteralHolder : IParamElement
{
    IParamLiteralHolder? ParentHolder { get; set; }
    List<IParamLiteral> Literals { get; set; }
}



