namespace BisUtils.Param.Models.Literals;

using System.Globalization;
using Core.IO;
using FResults;
using Options;
using Stubs;

public interface IParamFloat : IParamLiteral
{

}

public class ParamFloat : ParamLiteral<float>, IParamFloat
{
    public override byte LiteralId => 1;
    public override float Value { get; set; }

    public ParamFloat(IParamFile? file, float value) : base(file, value)
    {
    }

    public ParamFloat(IParamFile? file, BisBinaryReader reader, ParamOptions options) : base(file, reader, options)
    {
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        var result = base.Binarize(writer, options);
        writer.Write(Value);
        return LastResult = result;
    }

    public override Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        ParamValue = reader.ReadSingle();
        return LastResult = Result.ImmutableOk();
    }

    public override Result ToParam(out string str, ParamOptions options)
    {
        str = ParamValue?.ToString() ?? "";
        return LastResult = Result.ImmutableOk();
    }
}
