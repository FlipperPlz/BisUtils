namespace BisUtils.Param.Models.Stubs;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.IO;
using FResults;
using Options;

public interface IParamElement : IStrictBinaryObject<ParamOptions>
{
    IParamFile? ParamFile { get; }

    Result WriteParam(out string value, ParamOptions options);
}

public abstract class ParamElement : StrictBinaryObject<ParamOptions>, IParamElement
{
    public IParamFile? ParamFile { get; set; }

    protected ParamElement(IParamFile? file) => ParamFile = file;

    protected ParamElement(IParamFile? file, BisBinaryReader reader, ParamOptions options) : base(reader, options) => ParamFile = file;

    public abstract Result WriteParam(out string value, ParamOptions options);
}
