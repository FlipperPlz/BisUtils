namespace BisUtils.Extensions.RVBank.Extras;

using BisUtils.RVBank.Model.Entry;

public static class RVBankDirectoryTreeExtensions
{


    public static IEnumerable<string> DirectoryTree(this IRVBankDirectory directory, int indent = 0)
    {
        //write filetree

        yield return new string(' ', indent) + "├── " + directory.EntryName;

        foreach (var entry in directory.PboEntries)
        {
            switch (entry)
            {
                case IRVBankDataEntry dataEntry:
                    yield return new string(' ', indent + 2) + "├── " + dataEntry.EntryName;
                    break;

                case IRVBankDirectory directoryEntry:
                    foreach (var child in DirectoryTree(directoryEntry, indent + 2))
                    {
                        yield return child;
                    }
                    break;
            }
        }
    }
}
