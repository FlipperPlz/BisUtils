namespace BisUtils.RvConfig.Models.Stubs;

using Core.IO;
using FResults;
using Holders;
using Microsoft.Extensions.Logging;
using Options;

public interface IParamLiteral : IParamElement
{
    object? ParamValue { get; }
    byte LiteralId { get; }
    IParamLiteralHolder Parent { get; }

    void SyncToContext(IParamLiteralHolder holder);
}

public interface IParamLiteral<T> : IParamLiteral
{
    T Value { get; set; }
}

public abstract class ParamLiteral<T> : ParamElement, IParamLiteral<T>
{
    public abstract byte LiteralId { get; }
    public IParamLiteralHolder Parent { get; set; }
    public abstract T Value { get; set; }

    protected ParamLiteral(T? value, IRvConfigFile file, IParamLiteralHolder parent, ILogger? logger) : base(file, logger)
    {
        ParamValue = value;
        Parent = parent;
    }

    protected ParamLiteral(BisBinaryReader reader, ParamOptions options, IRvConfigFile file, IParamLiteralHolder parent, ILogger? logger)
        : base(reader, options, file, logger) =>
        Parent = parent;

    public object? ParamValue
    {
        get => Value;
        protected set
        {
            if (value is not T or null)
            {
                throw new NotSupportedException();
            }

            Value = (T)value;
        }
    }

    public void SyncToContext(IParamLiteralHolder holder)
    {
        Parent = holder;
        RvConfigFile = holder.RvConfigFile;
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
