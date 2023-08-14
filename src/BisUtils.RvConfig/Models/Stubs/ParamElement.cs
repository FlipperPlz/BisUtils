namespace BisUtils.RvConfig.Models.Stubs;

using System.Text;
using Core.Binarize;
using Core.Binarize.Implementation;
using Core.IO;
using FResults;
using Microsoft.Extensions.Logging;
using Options;

public interface IParamElement : IStrictBinaryObject<ParamOptions>
{
    IParamFile ParamFile { get; }

    Result WriteParam(ref StringBuilder builder, ParamOptions options);
}

public abstract class ParamElement : StrictBinaryObject<ParamOptions>, IParamElement
{
    public IParamFile ParamFile { get; set; }

    protected ParamElement(IParamFile file, ILogger? logger) : base(logger) => ParamFile = file;

    protected ParamElement(BisBinaryReader reader, ParamOptions options, IParamFile file, ILogger? logger) : base(reader, options, logger) =>
        ParamFile = file;

    public abstract Result WriteParam(ref StringBuilder builder, ParamOptions options);
}
