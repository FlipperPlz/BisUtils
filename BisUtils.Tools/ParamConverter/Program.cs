using System.Text;
using BisUtils.Extensions.ParamConversion;
using BisUtils.Extensions.ParamConversion.Enumeration;
using BisUtils.Parsers.ParamParser;

namespace ParamConverter; 

public static class Program {
    public static void Main(string[] arguments) {
        var paramFile = new ParamFile().FromBinary(File.OpenRead(@"C:\Program Files (x86)\Steam\steamapps\common\DayZ Tools\Bin\CfgConvert\test.bin"));
        File.WriteAllBytes(@"C:\Program Files (x86)\Steam\steamapps\common\DayZ Tools\Bin\CfgConvert\out.bin", paramFile.ToBinary().ToArray());
        File.WriteAllText(@"C:\Program Files (x86)\Steam\steamapps\common\DayZ Tools\Bin\CfgConvert\out.xml", paramFile.ToString(ParamFileTextFormats.XML), Encoding.UTF8);
        File.WriteAllText(@"C:\Program Files (x86)\Steam\steamapps\common\DayZ Tools\Bin\CfgConvert\out.cpp", paramFile.ToString(ParamFileTextFormats.CPP),  Encoding.UTF8);

    }
}