using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using BisUtils.PBO;
using BisUtils.PBO.Builders;
using BisUtils.PBO.Entries;
using Microsoft.Win32;

namespace UnitTests.BisUtils.PBO; 

public class StandardPBOTests {
    private readonly FileInfo TestConfig;
    private FileInfo? _bisFileBankExecutable;

    
    public readonly string BisUtilsTempDirectory = "BisUtils.UnitTests\\PBO";
    public readonly string DayZToolsNotInstalledError = "DayZ Tools is not installed, cannot create test data!";
    public readonly string[] RelativeFileBankLocation = {"PboUtils", "FileBank.exe"};

    public const string TestConfigData = 
@"class CfgPatches {
    class TestMod {
        requiredAddons[]= {""DZ_Data""};
    };
};
class CfgMods {
	class TestMod {
		type = ""mod"";
        dependencies[]={""Game""};
    };
};";


    public StandardPBOTests() {
        TestConfig = new FileInfo(Path.Combine(Environment.CurrentDirectory, "TestConfig.cpp"));
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) throw new Exception("Windows Only!");
        WriteTestData();
    }
    
    [Theory]
    [InlineData(new[] {(object) 1})]
    [InlineData(new[] {(object) 2})]
    [InlineData(new[] {(object) 3})]
    public void TestPboPacking(int entryCount) {
        var controlPboFileInfo = CreateControlPbo(entryCount);
        var createdPboFileInfo = CreateBisUtilsPbo(entryCount);
        
        var controlPbo = new PboFile(controlPboFileInfo.OpenRead());
        var createdPbo = new PboFile(createdPboFileInfo.OpenRead());
        Assert.Equal(createdPbo.GetPboEntries().Count(), entryCount + 2);
        Assert.Equal(createdPbo.GetPboEntries().Count(), controlPbo.GetPboEntries().Count());

        var controlDataEntries = controlPbo.GetPboEntries().Where(e => e is PboDataEntry).Cast<PboDataEntry>().ToList();
        var createdDataEntries = createdPbo.GetPboEntries().Where(e => e is PboDataEntry).Cast<PboDataEntry>().ToList();
        
        for (int i = 0; i < entryCount; i++) {
            Assert.Equal(createdDataEntries[i].EntryData, controlDataEntries[i].EntryData);
        }
    }

    private void WriteTestData() {
        if (TestConfig.Exists) TestConfig.Delete();
        using var writer = TestConfig.CreateText();
        writer.Write(TestConfigData);
    }

    public FileInfo CreateBisUtilsPbo(int dataCount = 1) {
        var createdPboFileInfo = new FileInfo(Path.Combine(Path.GetTempPath(), BisUtilsTempDirectory, "BisUtils.pbo"));
        var pbo = new PboBuilder(Path.GetTempFileName(), "TestMod");
        for (var i = 0; i < dataCount; i++) pbo.WithEntry($"{i}\\config.cpp", TestConfigData);
        pbo.GetPboFile().WriteBinary(new BinaryWriter(createdPboFileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite), Encoding.UTF8), PboBinarizationOptions.DefaultOptions);
        
        return createdPboFileInfo;
    }

    public FileInfo CreateControlPbo(int dataCount = 1) {
        var modRoot = new DirectoryInfo(Path.Combine(Path.GetTempPath(), BisUtilsTempDirectory, "TestMod"));
        var outputPbo = new FileInfo(Path.Combine(Path.GetTempPath(), BisUtilsTempDirectory, "control.pbo"));

        if(modRoot.Exists) modRoot.Delete();
        if(outputPbo.Exists) outputPbo.Delete();
        
        modRoot.Create();
        for (var i = 0; i < dataCount; i++) TestConfig.CopyTo(Path.Combine(modRoot.FullName, $"{i}\\config.cpp"));

        var processStartInfo = new ProcessStartInfo(GetFileBankExecutable().FullName);
        processStartInfo.Arguments = $"-property prefix=TestMod -dst {outputPbo.FullName} {modRoot.FullName}";
        var fileBankProcess = Process.Start(processStartInfo) ?? throw new Exception("Failed to execute FileBank.exe");
        fileBankProcess.WaitForExit();

        if (!outputPbo.Exists) throw new Exception("Failed to create control PBO using DayZ Tools!");

        return outputPbo;
    }
    
#pragma warning disable CA1416
    private FileInfo GetFileBankExecutable() {
        if (_bisFileBankExecutable is not null && _bisFileBankExecutable.Exists) return _bisFileBankExecutable;
        var key = 
            Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Bohemia Interactive\Dayz Tools\AddonBuilder") ??
            Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Bohemia Interactive\Dayz Tools\AddonBuilder");
        string? addonBuilderPath;
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        // addonBuilderPath is set in the if statement.
        if (key is null ||
            (addonBuilderPath = (string?) key.GetValue("path")) is null ||
            addonBuilderPath is null)
            throw new Exception(DayZToolsNotInstalledError);
        
        var path = new[] { new DirectoryInfo(addonBuilderPath).Parent!.FullName };
        _bisFileBankExecutable = new FileInfo(Path.Combine(path.Concat(RelativeFileBankLocation).ToArray()));

        if (!_bisFileBankExecutable.Exists) 
            throw new Exception($"Failed to locate {_bisFileBankExecutable.FullName}");

        return _bisFileBankExecutable;
    }
#pragma warning restore CA1416
}