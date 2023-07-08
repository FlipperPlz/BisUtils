namespace BisUtils.Param.Models.Stubs;

using Core.IO;
using FResults;
using Holders;
using Options;

public interface IParamLiteral : IParamElement
{
    object? ParamValue { get; }
    byte LiteralId { get; }
    IParamLiteralHolder? Parent { get; }

    void SyncToContext(IParamLiteralHolder? holder);
}

public interface IParamLiteral<out T> : IParamLiteral
{
    T? Value { get; }
}

public abstract class ParamLiteral<T> : ParamElement, IParamLiteral<T>
{
    protected ParamLiteral(IParamFile? file, IParamLiteralHolder? parent, T? value) : base(file)
    {
        ParamValue = value;
        Parent = parent;
    }

    protected ParamLiteral(IParamFile? file, IParamLiteralHolder? parent, BisBinaryReader reader, ParamOptions options)
        : base(file, reader, options) =>
        Parent = parent;

    public abstract byte LiteralId { get; }
    public IParamLiteralHolder? Parent { get; set; }
    public abstract T? Value { get; set; }

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

    public void SyncToContext(IParamLiteralHolder? holder)
    {
        Parent = holder;
        ParamFile = holder?.ParamFile;
    }

    public override Result Validate(ParamOptions options) =>
        LastResult = ParamValue is T ? Result.Ok() : Result.Fail("Wrong value type");

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
