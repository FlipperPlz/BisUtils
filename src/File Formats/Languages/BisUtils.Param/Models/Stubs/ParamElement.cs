namespace BisUtils.Param.Models.Stubs;

using System.Text;
using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Family;
using Core.IO;
using FResults;
using Options;

public interface IParamElement : IStrictBinaryObject<ParamOptions>
{
    IParamFile? ParamFile { get; }

    Result WriteParam(StringBuilder builder, ParamOptions options);
    StringBuilder WriteParam(out Result result, ParamOptions options);
    Result ToParam(out string str, ParamOptions options);
    string ToParam(ParamOptions options);
}

public abstract class ParamElement : StrictBinaryObject<ParamOptions>, IParamElement
{
    public IParamFile? ParamFile { get; set; }

    protected ParamElement(IParamFile? file) => ParamFile = file;

    protected ParamElement(IParamFile? file, BisBinaryReader reader, ParamOptions options) : base(reader, options) => ParamFile = file;

    public abstract Result ToParam(out string str, ParamOptions options);

    public string ToParam(ParamOptions options)
    {
        ToParam(out var str, options);
        return str;
    }

    public Result WriteParam(StringBuilder builder, ParamOptions options)
    {
        var result = ToParam(out var str, options);
        builder.Append(str);
        return result;
    }

    public StringBuilder WriteParam(out Result result, ParamOptions options)
    {
        var builder = new StringBuilder();
        result = WriteParam(builder, options);
        return builder;
    }
}
