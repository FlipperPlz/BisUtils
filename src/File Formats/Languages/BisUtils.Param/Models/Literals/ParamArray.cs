namespace BisUtils.Param.Models.Literals;

using System.Text;
using Core.Extensions;
using Core.IO;
using Extensions;
using Factories;
using FResults;
using FResults.Extensions;
using FResults.Reasoning;
using Options;
using Stubs;
using Stubs.Holders;

public interface IParamArray : IParamLiteral<List<IParamLiteral>>, IParamLiteralHolder
{
}
// ParamArray is reserved? why...
#pragma warning disable CA1716
public class ParamArray : ParamLiteral<List<IParamLiteral>>, IParamArray
{
    public ParamArray(IParamFile? file, IParamLiteralHolder? parent, List<IParamLiteral> value) : base(file, parent,
        value)
    {
    }

    public ParamArray(IParamFile? file, IParamLiteralHolder? parent, IEnumerable<IParamLiteral> value) : this(file,
        parent, value.ToList())
    {
    }

    public ParamArray(IParamFile? file, IParamLiteralHolder? parent, BisBinaryReader reader, ParamOptions options) :
        base(file, parent, reader, options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override byte LiteralId => 3;
    public override List<IParamLiteral> Value { get; set; } = new List<IParamLiteral>();
    public IParamLiteralHolder? ParentHolder => Parent;
    public List<IParamLiteral> Literals => Value;

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        var result = base.Binarize(writer, options);
        var value = Value;
        writer.WriteCompactInteger(value.Count);
        return LastResult = result.WithReasons(value?.SelectMany(v => v.Binarize(writer, options).Reasons) ??
                                               new IReason[] { Result.Fail("Failed to write data in list"), });
    }

    public sealed override Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        var results = Result.Ok();
        var contents = new List<IParamLiteral>(reader.ReadCompactInteger());
        for (var i = 0; i < contents.Capacity; ++i)
        {
            results.WithReasons(ParamLiteralFactory.ReadLiteral(ParamFile, this, reader, options, out var literal)
                .Reasons);
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
        LastResult = base.Validate(options).IsFailed
            ? LastResult!
            : LastResult!.WithReasons(Value.SelectMany(e => e.Validate(options).Reasons) ?? Array.Empty<IReason>());

    public override Result WriteParam(out string value, ParamOptions options)
    {
        value = Value.Any()
            ? $"{{{string.Join(", ", Value.Select(v => v.ToParam(out _, options)))}}}"
            : "{}";
        return LastResult = Result.Ok();
    }

    public static implicit operator List<IParamLiteral>(ParamArray array) => array.Value;

    public static explicit operator ParamArray(List<IParamLiteral> array) => new(null, null, array);

    public Result WriteLiterals(out string value, ParamOptions options)
    {
        value = string.Join(',', Literals.Select(s => s.ToParam(out _, options)));
        return Result.Ok();
    }

}
