namespace BisUtils.Param.Models.Stubs;

using System.Text;
using Core.IO;
using FResults;
using Literals;
using Options;

public interface IParamLiteral : IParamElement
{
    object? ParamValue { get; }
    byte LiteralId { get; }
}


public static class ParamLiteral
{
    public static Result DebinarizeLiteral(IParamFile? file, BisBinaryReader reader, ParamOptions options, out IParamLiteral? literal)
    {
        var id = reader.ReadByte();
        literal = id switch
        {
            0 => new ParamString(file, reader, options),
            1 => new ParamFloat(file, reader, options),
            2 => new ParamInt(file, reader, options),
            3 => new ParamArray(file, reader, options),
            _ => null
        };
        if (literal is null)
        {
            return Result.Fail($"Unknown Literal ID '{id}'.");
        }

        return literal.LastResult ?? Result.Ok();
    }
}

public abstract class ParamLiteral<T> : ParamElement, IParamLiteral
{
    public object? ParamValue
    {
        get => Value;
        protected set
        {
            if (value is not T or null)
            {
                throw new NotSupportedException();
            }

            Value = (T?)value;
        }
    }
    public abstract byte LiteralId { get; }
    public abstract T? Value { get; set; }

    protected ParamLiteral(IParamFile? file, T? value) : base(file) => ParamValue = value;

    protected ParamLiteral(IParamFile? file, BisBinaryReader reader, ParamOptions options) : base(file, reader, options)
    {
    }

    public override Result Validate(ParamOptions options) => LastResult = ParamValue is T ? Result.Ok() : Result.Fail("Wrong value type");

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        if (options.WriteLiteralId)
        {
            writer.Write(LiteralId);
        }
        return Result.Ok();
    }

    public abstract override Result Debinarize(BisBinaryReader reader, ParamOptions options);
}
