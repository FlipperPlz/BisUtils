using System.Text;
using BisUtils.PBO;
using BisUtils.PBO.Builders;
using Microsoft.Win32;

namespace UnitTests.BisUtils.PBO; 

public class StandardPBOTests {
    private DirectoryInfo TestingDirectory;

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

    [Fact]
    public void TestPboSyncing_WhenDeletingEntries() {
        TestingDirectory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "PBOTesting"));
        if (!TestingDirectory.Exists) TestingDirectory.Create();
        var testingPBO = Path.Combine(TestingDirectory.FullName, "TestPboSyncing_WhenDeletingEntries.pbo");
        if(File.Exists(testingPBO)) File.Delete(testingPBO);
        var pbo = new PboFile(
            testingPBO,
            PboFileOption.Create);
        pbo.AddEntry(new PboDataEntryDto(pbo, new MemoryStream(Encoding.UTF8.GetBytes(TestConfigData))) {
            EntryName = "config.cpp"
        }, true);
        pbo.DeleteEntry(pbo.GetDataEntries().First(), true);
        pbo.Dispose();
        pbo = new PboFile(testingPBO);
        Assert.Empty(pbo.GetDataEntries());
        pbo.Dispose();
        File.Delete(testingPBO);
    }

    [Fact]
    public void TestPboSyncing_WhenAddingEntries() {
        TestingDirectory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "PBOTesting"));
        if (!TestingDirectory.Exists) TestingDirectory.Create();
        var testingPBO = Path.Combine(TestingDirectory.FullName, "TestPboSyncing_WhenAddingEntries.pbo");
        if(File.Exists(testingPBO)) File.Delete(testingPBO);
        var pbo = new PboFile(
            testingPBO,
            PboFileOption.Create);
        pbo.AddEntry(new PboDataEntryDto(pbo, new MemoryStream(Encoding.UTF8.GetBytes(TestConfigData))) {
            EntryName = "config.cpp"
        }, true);
        pbo.Dispose();
        pbo = new PboFile(testingPBO);
        foreach (var pboDataEntry in pbo.GetDataEntries()) {
            if(pboDataEntry.EntryName is not "config.cpp") continue;
            Assert.Equal(Encoding.UTF8.GetBytes(TestConfigData), pboDataEntry.EntryData);
        }
        pbo.Dispose();
        File.Delete(testingPBO);
    }
    
    [Fact]
    public void TestPboSyncing_WhenEditingEntries() {
        var editedData = new byte[] {0x45, 0x6e, 0x74, 0x72, 0x79, 0x20, 0x45, 0x64, 0x69, 0x74, 0x65, 0x64};
        TestingDirectory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "PBOTesting"));
        if (!TestingDirectory.Exists) TestingDirectory.Create();
        var testingPBO = Path.Combine(TestingDirectory.FullName, "TestPboSyncing_WhenEditingEntries.pbo");
        if(File.Exists(testingPBO)) File.Delete(testingPBO);
        var pbo = new PboFile(
            testingPBO,
            PboFileOption.Create);
        var addedEntry = new PboDataEntryDto(pbo, new MemoryStream(Encoding.UTF8.GetBytes(TestConfigData))) {
            EntryName = "config.cpp"
        };
        pbo.AddEntry(addedEntry, true);
        foreach (var pboDataEntry in pbo.GetDataEntries()) {
            pbo.OverwriteEntryData(pboDataEntry, editedData, syncStream: true );
        }
        
        pbo.Dispose();
        pbo = new PboFile(testingPBO);
        foreach (var pboDataEntry in pbo.GetDataEntries()) {
            if(pboDataEntry.EntryName is not "config.cpp") continue;
            Assert.Equal(editedData.Length, pboDataEntry.EntryData.Length);
            Assert.Equal(editedData, pboDataEntry.EntryData);
        }
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