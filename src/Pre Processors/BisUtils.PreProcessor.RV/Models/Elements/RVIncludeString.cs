namespace BisUtils.PreProcessor.RV.Models.Elements;

using Enumerations;
using FResults;
using Stubs;

public interface IRVIncludeString : IRVElement
{
    string Value { get; }
    RVStringType StringType { get; }
}

public class RVIncludeString: RVElement, IRVIncludeString
{
    public string Value { get; set; }
    public RVStringType StringType { get; set; }

    public RVIncludeString(IRVPreProcessor preProcessor, string value, RVStringType stringType = RVStringType.Angled) : base(preProcessor)
    {
        Value = value;
        StringType = stringType;
    }


    public override Result ToText(out string str) => throw new NotImplementedException();

}
