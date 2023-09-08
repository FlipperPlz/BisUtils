namespace BisUtils.RvBank.ExtraExtensions;

using Model.Entry;

public static class RvBankDirectoryTreeExtensions
{
    public static IEnumerable<string> DirectoryTree(this IRvBankDirectory directory, int indent = 0)
    {
        yield return new string(' ', indent) + "├── " + directory.EntryName;

        foreach (var entry in directory.PboEntries)
        {
            switch (entry)
            {
                case IRvBankDataEntry dataEntry:
                    yield return new string(' ', indent + 2) + "├── " + dataEntry.EntryName;
                    break;

                case IRvBankDirectory directoryEntry:
                    foreach (var child in DirectoryTree(directoryEntry, indent + 2))
                    {
                        yield return child;
                    }
                    break;
            }
        }
    }
}
