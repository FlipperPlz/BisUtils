namespace BisUtils.Param.Models.Literals;

using System.Globalization;
using Core.IO;
using FResults;
using Options;
using Stubs;

public interface IParamInt : IParamNumericLiteral<int>
{

}

public struct ParamInt : IParamInt
{
    public IParamFile? ParamFile { get; set; }
    public Result? LastResult { get; private set; }
    public required int ParamValue { get; set; }

    public Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        writer.Write(ParamValue);
        return LastResult = Result.ImmutableOk();
    }

    public Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        ParamValue = reader.ReadInt32();
        return LastResult = Result.ImmutableOk();
    }

    public Result Validate(ParamOptions options) =>
        LastResult = Result.ImmutableOk();


    public Result ToParam(out string str, ParamOptions options)
    {
        str = ParamValue.ToString("D", CultureInfo.CurrentCulture);
        return LastResult = Result.ImmutableOk();
    }
}
