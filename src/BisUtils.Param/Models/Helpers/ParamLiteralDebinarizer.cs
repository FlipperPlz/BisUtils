namespace BisUtils.Param.Models.Helpers;

using Core.IO;
using FResults;
using Options;
using Stubs;


public class ParamLiteralDebinarizer
{

    //TODO: Abstract to options get byte from LiteralIdFosters
    public static Result DebinarizeLiteral(out IParamLiteralBase literal, BisBinaryReader reader, ParamOptions options)
    {
        var id = reader.ReadByte();

        throw new NotImplementedException();
    }

}
