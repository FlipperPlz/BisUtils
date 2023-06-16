
using BisUtils.Bank;
using BisUtils.Bank.Model;
using BisUtils.Core.IO;

class MyClass
{
    public static void Main()
    {

        var pbo = new PboFile(new BisBinaryReader(File.OpenRead(@"C:\Users\developer\Downloads\HM_Core.pbo")), new PboOptions());

        Console.WriteLine("");
    }
}
