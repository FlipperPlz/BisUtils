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
    IRvConfigFile RvConfigFile { get; }

    Result WriteParam(ref StringBuilder builder, ParamOptions options);
}

public abstract class ParamElement : StrictBinaryObject<ParamOptions>, IParamElement
{
    public IRvConfigFile RvConfigFile { get; set; } = null!;

    protected ParamElement(IRvConfigFile file, ILogger? logger) : base(logger) => RvConfigFile = file;

    protected ParamElement(BisBinaryReader reader, ParamOptions options, IRvConfigFile file, ILogger? logger) : base(reader, options, logger) =>
        RvConfigFile = file;

    protected ParamElement() : base(default)
    {

    }

    public abstract Result WriteParam(ref StringBuilder builder, ParamOptions options);
}
