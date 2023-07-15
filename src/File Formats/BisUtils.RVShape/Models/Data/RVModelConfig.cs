namespace BisUtils.RVShape.Models.Data;

using BisUtils.Core.IO;
using BisUtils.Param.Models;
using BisUtils.Param.Models.Statements;
using BisUtils.Param.Models.Stubs;
using BisUtils.Param.Options;

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

    public RVModelConfig(string modelName, List<IParamStatement>? statements = null) : base(modelName, statements)
    {
        SkeletonConfigs = this >> "CfgSkeletons";
        ModelConfigs = this >> "CfgModels";
        ModelConfig = ModelConfigs is not null ? ModelConfigs >> ModelName : null;
    }

    public RVModelConfig(string modelName, BisBinaryReader reader, ParamOptions options) : base(modelName, reader,
        options)
    {
        SkeletonConfigs = this >> "CfgSkeletons";
        ModelConfigs = this >> "CfgModels";
        ModelConfig = ModelConfigs is not null ? ModelConfigs >> ModelName : null;
    }
}
