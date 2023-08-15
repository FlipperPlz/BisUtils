namespace BisUtils.RvShape.Models.Data;

using Core.IO;
using RvConfig.Models;
using RvConfig.Models.Statements;
using RvConfig.Models.Stubs;
using RvConfig.Options;
using Microsoft.Extensions.Logging;

public interface IRvModelConfig : IRvConfigFile
{
    public string ModelName { get; }
    public IParamClass? SkeletonConfigs { get; }
    public IParamClass? ModelConfigs { get; }
    public IParamClass? ModelConfig { get; }
}

public class RvModelConfig : RvConfigFile, IRvModelConfig
{
    public string ModelName => FileName;
    public IParamClass? SkeletonConfigs { get; }
    public IParamClass? ModelConfigs { get; }
    public IParamClass? ModelConfig { get; }

    public RvModelConfig(string modelName, List<IParamStatement> statements, ILogger? logger) : base(modelName, statements, logger)
    {
        SkeletonConfigs = null;
        ModelConfigs = null;
        ModelConfig = ModelConfigs is not null ? ModelConfigs >> ModelName : null;
    }

    public RvModelConfig(string modelName, BisBinaryReader reader, ParamOptions options, ILogger? logger) : base(modelName, reader,
        options, logger)
    {
        SkeletonConfigs = null;
        ModelConfigs = null;
        ModelConfig = ModelConfigs is not null ? ModelConfigs >> ModelName : null;
    }
}
