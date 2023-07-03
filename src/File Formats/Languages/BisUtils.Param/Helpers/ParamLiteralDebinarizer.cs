namespace BisUtils.Param.Helpers;

using BisUtils.Core.IO;
using BisUtils.Param.Models.Stubs;
using BisUtils.Param.Options;
using FResults;

public class ParamLiteralDebinarizer
{

    //TODO: Abstract to options get byte from LiteralIdFosters
    public static Result DebinarizeLiteral(out IParamLiteralBase literal, BisBinaryReader reader, ParamOptions options)
    {
        var id = reader.ReadByte();

        throw new NotImplementedException();
    }

}
