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
    StringBuilder WriteParam(ParamOptions options);
    string ToParam(ParamOptions options);
}

public abstract class ParamElement : StrictBinaryObject<ParamOptions>, IParamElement
{
    protected ParamElement(IParamFile? file) : base()
    {
    }

    protected ParamElement(BisBinaryReader reader, ParamOptions options) : base(reader, options)
    {
    }

    public IFamilyNode? Node => ParamFile;
    public IParamFile? ParamFile { get; set; }

    public abstract Result WriteParam(StringBuilder builder, ParamOptions options);

    public StringBuilder WriteParam(ParamOptions options)
    {
        var builder = new StringBuilder();
        WriteParam(builder, options);
        return builder;
    }

    public string ToParam(ParamOptions options) =>
        WriteParam(options).ToString();


}
