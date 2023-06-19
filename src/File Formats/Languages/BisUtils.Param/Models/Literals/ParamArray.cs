namespace BisUtils.Param.Models.Literals;

using Core.IO;
using FResults;
using Helpers;
using Options;
using Stubs;

public interface IParamArray : IParamLiteral<List<IParamLiteralBase>>
{

}
// ParamArray is reserved? why...
#pragma warning disable CA1716
public struct ParamArray : IParamArray
{
    public ParamArray()
    {
    }

    public IParamFile? ParamFile { get; set; } = null;
    public Result? LastResult { get; private set; } = null;
    public required List<IParamLiteralBase> ParamValue { get; set; } = new();

    public Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        writer.Write(options.LiteralIdFoster(GetType()));
        writer.WriteCompactInteger(ParamValue.Count);

        return Result.Merge(ParamValue.Select(v => v.Binarize(writer, options)));
    }

    public Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        var results = new List<Result>();
        ParamValue = new List<IParamLiteralBase>(reader.ReadCompactInteger());
        for (var i = 0; i < ParamValue.Capacity; ++i)
        {
            results.Add(ParamLiteralDebinarizer.DebinarizeLiteral(out var literal, reader, options));
            ParamValue.Add(literal);
        }

        return LastResult = Result.Merge(results);
    }

    public Result Validate(ParamOptions options) =>
        LastResult = Result.Merge(ParamValue.Select(v => v.Validate(options)));

    public Result ToParam(out string str, ParamOptions options)
    {

        str = $"{{{string.Join(", ", ParamValue.Select(v => v.ToParam(options)))}}}";
        return LastResult = Result.Merge
        (
            ParamValue.Where(v => v.LastResult is not null)
                .Select(v => v.LastResult!)
        );
    }
}
