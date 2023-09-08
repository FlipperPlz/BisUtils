namespace BisUtils.RvConfig.Utils;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Extensions;
using Core.IO;
using FResults;
using Microsoft.Extensions.Logging;
using Models;
using Models.Stubs;
using Options;

public interface IParamImplementation : IBinaryObject<ParamOptions>
{
    IRvConfigFile RvConfigFile { get; set; }
    string Name { get; set; }
}

public class ParamImplementation : BinaryObject<ParamOptions>, IParamImplementation
{
    public IRvConfigFile RvConfigFile { get; set; }
    public string Name { get => RvConfigFile.FileName; set => RvConfigFile.FileName = value; }

    protected ParamImplementation(IRvConfigFile rvConfigFile) : base(((ParamElement)rvConfigFile).Logger) => RvConfigFile = rvConfigFile;

    protected ParamImplementation(string name, BisBinaryReader reader, ParamOptions options, ILogger? logger) : base(logger)
    {
        RvConfigFile = new RvConfigFile(name, reader, options, logger);
        if (RvConfigFile.LastResult is { } result && result)
        {
            result.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options) =>
       LastResult = RvConfigFile.Binarize(writer, options);

    public sealed override Result Debinarize(BisBinaryReader reader, ParamOptions options) =>
        LastResult = RvConfigFile.Debinarize(reader, options);
}
