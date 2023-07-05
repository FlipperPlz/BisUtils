namespace BisUtils.Param.Models.Literals;

using System.Globalization;
using Core.IO;
using FResults;
using Options;
using Stubs;

public interface IParamInt : IParamLiteral
{

}

public class ParamInt : ParamLiteral<int>, IParamInt
{

    public override byte LiteralId { get; }
    public override int Value { get; set; }

    public ParamInt(IParamFile? file, int value) : base(file, value)
    {
    }

    public ParamInt(IParamFile? file, BisBinaryReader reader, ParamOptions options) : base(file, reader, options)
    {
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        var result = base.Binarize(writer, options);
        writer.Write(Value);
        return result;
    }

    public override Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        ParamValue = reader.ReadInt32();
        return LastResult = Result.ImmutableOk();
    }


    public override Result Validate(ParamOptions options) =>
        LastResult = Result.ImmutableOk();


    public override Result ToParam(out string str, ParamOptions options)
    {
        str = Value.ToString("D", CultureInfo.CurrentCulture);
        return LastResult = Result.ImmutableOk();
    }


}
