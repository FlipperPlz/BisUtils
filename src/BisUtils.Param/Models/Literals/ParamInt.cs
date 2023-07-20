namespace BisUtils.Param.Models.Literals;

using System.Globalization;
using System.Text;
using Core.Extensions;
using Core.IO;
using FResults;
using Options;
using Stubs;
using Stubs.Holders;

public interface IParamInt : IParamLiteral<int>
{

}

public class ParamInt : ParamLiteral<int>, IParamInt
{
    public override byte LiteralId => 2;
    public override int Value { get; set; }

    public ParamInt(IParamFile file, IParamLiteralHolder parent, int value) : base(file, parent, value)
    {
    }

    public ParamInt(IParamFile file, IParamLiteralHolder parent, BisBinaryReader reader, ParamOptions options) : base(file, parent, reader, options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        var result = base.Binarize(writer, options);
        writer.Write(Value);
        return result;
    }

    public sealed override Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        ParamValue = reader.ReadInt32();
        return LastResult = Result.Ok();
    }


    public override Result Validate(ParamOptions options) =>
        LastResult = Result.Ok();


    public override Result WriteParam(ref StringBuilder builder, ParamOptions options)
    {
        builder.Append(Value.ToString("D", CultureInfo.CurrentCulture));
        return LastResult = Result.Ok();
    }
}
