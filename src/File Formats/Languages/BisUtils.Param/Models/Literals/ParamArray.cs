namespace BisUtils.Param.Models.Literals;

using System.Text;
using Core.Extensions;
using Core.IO;
using Factories;
using FResults;
using FResults.Extensions;
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
    public override byte LiteralId => 3;
    public override List<IParamLiteral> Value { get; set; } = new List<IParamLiteral>();
    public IParamLiteralHolder? ParentHolder { get; set; } = null!;
    public List<IParamLiteral> Literals { get => Value; set => Value = value; }

    public ParamArray(IParamFile file, IParamLiteralHolder parent, List<IParamLiteral> value) : base(file, parent,
        value)
    {
    }

    public ParamArray(IParamFile file, IParamLiteralHolder parent, IEnumerable<IParamLiteral> value) : this(file,
        parent, value.ToList())
    {
    }

    public ParamArray(IParamFile file, IParamLiteralHolder parent, BisBinaryReader reader, ParamOptions options) :
        base(file, parent, reader, options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        var result = base.Binarize(writer, options);
        var value = Value;
        writer.WriteCompactInteger(value.Count);
        return LastResult = result.WithReasons(value.SelectMany(v => v.Binarize(writer, options).Reasons));
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
            : LastResult!.WithReasons(Value.SelectMany(e => e.Validate(options).Reasons));


    public override Result WriteParam(ref StringBuilder builder, ParamOptions options)
    {
        builder.Append('{');
        var max = Value.Count;
        for (var i = 0; i < max; )
        {
            Value[i].WriteParam(ref builder, options);
            if(++i != max )
            {
                builder.Append(',');
            }
        }

        builder.Append('}');
        return LastResult = Result.Ok();
    }

    public static implicit operator List<IParamLiteral>(ParamArray array) => array.Value;

    public static explicit operator ParamArray(List<IParamLiteral> array) => new(null!, null!, array);

}
