namespace BisUtils.Param.Models.Literals;

using Core.Extensions;
using Core.IO;
using FResults;
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

    public ParamFloat(IParamFile? file, IParamLiteralHolder? parent, float value) : base(file, parent, value)
    {
    }

    public ParamFloat(IParamFile? file, IParamLiteralHolder? parent, BisBinaryReader reader, ParamOptions options) : base(file, parent, reader, options)
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

    public override Result WriteParam(out string value, ParamOptions options)
    {
        value = ParamValue?.ToString() ?? "";
        return LastResult = Result.ImmutableOk();
    }
}
