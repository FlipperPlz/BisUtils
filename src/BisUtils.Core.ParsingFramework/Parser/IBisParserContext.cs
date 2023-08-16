namespace BisUtils.Core.ParsingFramework.Parser;

public interface IBisParserContext
{
    public bool ShouldEnd { get; set; }


    public void MarkEnd() => ShouldEnd = true;
}
