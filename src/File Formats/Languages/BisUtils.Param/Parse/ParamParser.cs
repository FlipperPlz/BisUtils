namespace BisUtils.Param.Parse;

using BisUtils.Param.Models.Stubs;
using Models;

public static class ParamLiteralParser
{

    public static ParamFile Parse(string contents, string filename)
    {
        var paramFile = new ParamFile(filename, new List<IParamStatement>());

        throw new NotImplementedException();
    }
}
