namespace BisUtils.RvProcess.Models.Stubs;

using System.Text;
using FResults;

public interface IRvElement
{
    IRvPreProcessor PreProcessor { get; }

    Result WriteText(StringBuilder builder);
    StringBuilder WriteText(out Result result);
    Result ToText(out string str);
    string ToText();
}

public abstract class RvElement : IRvElement
{
    public IRvPreProcessor PreProcessor { get; }

    protected RvElement(IRvPreProcessor preProcessor) => PreProcessor = preProcessor;

    public abstract Result ToText(out string str);

    public Result WriteText(StringBuilder builder)
    {
        var result = ToText(out var str);
        builder.Append(str);
        return result;
    }

    public StringBuilder WriteText(out Result result)
    {
        var builder = new StringBuilder();
        result = WriteText(builder);
        return builder;
    }

    public string ToText()
    {
        ToText(out var str);
        return str;
    }
}
