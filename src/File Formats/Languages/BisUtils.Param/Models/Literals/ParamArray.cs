namespace BisUtils.Param.Models.Literals;

using Core.IO;
using FResults;
using FResults.Extensions;
using FResults.Reasoning;
using Options;
using Stubs;

public interface IParamArray : IParamLiteral
{

}
// ParamArray is reserved? why...
#pragma warning disable CA1716
public class ParamArray : ParamLiteral<List<IParamLiteral>>, IParamArray
{
    public override byte LiteralId => 3;
    public override List<IParamLiteral>? Value { get; set; } = new List<IParamLiteral>();

    public ParamArray(IParamFile? file, IEnumerable<IParamLiteral>? value) : base(file, value?.ToList())
    {
    }

    public ParamArray(IParamFile? file, BisBinaryReader reader, ParamOptions options) : base(file, reader, options)
    {
    }


    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        var result = base.Binarize(writer, options);
        var value = Value;
        writer.WriteCompactInteger(value?.Count ?? 0);
        return LastResult = result.WithReasons(value?.SelectMany(v => v.Binarize(writer, options).Reasons) ?? new IReason[] {Result.Fail("Failed to write data in list"), });
    }

    public override Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        var results = Result.Ok();
        var contents = new List<IParamLiteral>(reader.ReadCompactInteger());
        for (var i = 0; i < contents.Capacity ; ++i)
        {
            results.WithReasons(ParamLiteral.DebinarizeLiteral(ParamFile, reader, options, out var literal).Reasons);
            if (literal is null)
            {
                return results;
            }
            contents.Add(literal);
        }
        Value = contents;
        return LastResult = results;
    }

    public override Result Validate(ParamOptions options) =>
        LastResult = base.Validate(options).IsFailed ? LastResult! : LastResult!.WithReasons(Value?.SelectMany(e => e.Validate(options).Reasons) ?? Array.Empty<IReason>());

    public override Result ToParam(out string str, ParamOptions options)
    {
        str = Value is not null && Value.Any()
            ? $"{{{string.Join(", ", Value.Select(v => v.ToParam(options)))}}}"
            : "";
        return LastResult = Result.Ok();
    }


}
