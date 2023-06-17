
using BisUtils.Bank;
using BisUtils.Bank.Model;
using BisUtils.Bank.Model.Stubs;
using BisUtils.Core.IO;

class MyClass
{
    public static void Main()
    {

        var pbo = new PboFile(new List<IPboEntry>());
        var result = pbo.Debinarize(new BisBinaryReader(File.OpenRead(@"C:\Users\developer\Downloads\HM_Core.pbo")),
            new PboOptions());

        Console.WriteLine("");
    }
}
