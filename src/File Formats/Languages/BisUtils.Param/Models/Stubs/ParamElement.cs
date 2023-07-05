namespace BisUtils.Param.Models.Stubs;

using System.Text;
using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Family;
using Core.IO;
using FResults;
using Options;

public interface IParamElement : IFamilyMember, IStrictBinaryObject<ParamOptions>
{
    IParamFile? ParamFile { get; }
    IFamilyNode? IFamilyMember.Node => ParamFile;

    Result WriteParam(StringBuilder builder, ParamOptions options);
    StringBuilder WriteParam(out Result result, ParamOptions options);
    Result ToParam(out string str, ParamOptions options);

    string ToParam(ParamOptions options);
}

public abstract class ParamElement : StrictBinaryObject<ParamOptions>, IParamElement
{
    public IParamFile? ParamFile { get; set; }
    public IFamilyNode? Node => ParamFile;

    protected ParamElement(IParamFile? file) : base()
    {
    }

    protected ParamElement(IParamFile? file, BisBinaryReader reader, ParamOptions options) : base(reader, options)
    {
    }

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
