namespace BisUtils.Param.Models.Stubs;

using System.Text;
using Core.IO;
using FResults;
using Holders;
using Literals;
using Options;

public interface IParamLiteral : IParamElement
{
    object? ParamValue { get; }
    byte LiteralId { get; }
    IParamLiteralHolder? Parent { get; }
}


public static class ParamLiteral
{
    public static Result DebinarizeLiteral(IParamFile? file, IParamLiteralHolder? parent, BisBinaryReader reader, ParamOptions options, out IParamLiteral? literal)
    {
        var id = reader.ReadByte();
        literal = id switch
        {
            0 => new ParamString(file, parent, reader, options),
            1 => new ParamFloat(file, parent, reader, options),
            2 => new ParamInt(file, parent, reader, options),
            3 => new ParamArray(file, parent, reader, options),
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
    public IParamLiteralHolder? Parent { get; set; }
    public abstract T? Value { get; set; }

    protected ParamLiteral(IParamFile? file, IParamLiteralHolder? parent, T? value) : base(file)
    {
        ParamValue = value;
        Parent = parent;
    }

    protected ParamLiteral(IParamFile? file, IParamLiteralHolder? parent, BisBinaryReader reader, ParamOptions options) : base(file, reader, options) =>
        Parent = parent;

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
