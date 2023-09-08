namespace ParamTest;

using BisUtils.Core.Binarize.Flagging;
using BisUtils.RvConfig.Models;
using BisUtils.RvConfig.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

public class Foo
{
    public static void Main()
    {
        var config = File.OpenRead(@"C:\Program Files (x86)\Steam\steamapps\common\DayZ Tools\Bin\CfgConvert\config.cpp");
        var param = RvConfigFile.ReadParamFile("config", config, new ParamOptions(), NullLogger.Instance);

    }
}

