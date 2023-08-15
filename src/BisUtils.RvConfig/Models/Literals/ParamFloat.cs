namespace BisUtils.RvConfig.Models.Literals;

using System.Text;
using Core.Extensions;
using Core.IO;
using FResults;
using Microsoft.Extensions.Logging;
using Options;
using Stubs;
using Stubs.Holders;

public interface IParamFloat : IParamLiteral<float>
{

}

public class ParamFloat : ParamLiteral<float>, IParamFloat
{
    public override byte LiteralId => 1;
    public override float Value { get; set; }

    public ParamFloat(float value, IRvConfigFile file, IParamLiteralHolder parent, ILogger? logger) : base(value, file, parent, logger)
    {
    }

    public ParamFloat(BisBinaryReader reader, ParamOptions options, IRvConfigFile file, IParamLiteralHolder parent, ILogger? logger) : base(reader, options, file, parent, logger)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        var result = base.Binarize(writer, options);
        writer.Write(Value);
        return LastResult = result;
    }

    public sealed override Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        ParamValue = reader.ReadSingle();
        return LastResult = Result.ImmutableOk();
    }

    public override Result WriteParam(ref StringBuilder builder, ParamOptions options)
    {
        builder.Append(Value);
        return LastResult = Result.Ok();
    }

}
