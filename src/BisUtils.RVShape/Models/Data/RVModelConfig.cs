namespace BisUtils.RVShape.Models.Data;

using Core.IO;
using BisUtils.Param.Models;
using BisUtils.Param.Models.Statements;
using BisUtils.Param.Models.Stubs;
using BisUtils.Param.Options;
using Microsoft.Extensions.Logging;

public interface IRVModelConfig : IParamFile
{
    public string ModelName { get; }
    public IParamClass? SkeletonConfigs { get; }
    public IParamClass? ModelConfigs { get; }
    public IParamClass? ModelConfig { get; }
}

public class RVModelConfig : ParamFile, IRVModelConfig
{
    public string ModelName => FileName;
    public IParamClass? SkeletonConfigs { get; }
    public IParamClass? ModelConfigs { get; }
    public IParamClass? ModelConfig { get; }

    public RVModelConfig(string modelName, List<IParamStatement> statements, ILogger? logger) : base(modelName, statements, logger)
    {
        SkeletonConfigs = null;
        ModelConfigs = null;
        ModelConfig = ModelConfigs is not null ? ModelConfigs >> ModelName : null;
    }

    public RVModelConfig(string modelName, BisBinaryReader reader, ParamOptions options, ILogger? logger) : base(modelName, reader,
        options, logger)
    {
        SkeletonConfigs = null;
        ModelConfigs = null;
        ModelConfig = ModelConfigs is not null ? ModelConfigs >> ModelName : null;
    }
}
