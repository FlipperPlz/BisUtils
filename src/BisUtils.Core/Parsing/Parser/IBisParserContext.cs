namespace BisUtils.Core.Parsing.Parser;

public interface IBisParserContext
{
    public bool ShouldEnd { get; set; }


    public void MarkEnd() => ShouldEnd = true;
}
