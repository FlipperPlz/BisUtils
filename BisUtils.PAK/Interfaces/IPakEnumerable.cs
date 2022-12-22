using BisUtils.PAK.Entries;

namespace BisUtils.PAK.Interfaces; 

public interface IPakEnumerable {
    public List<PakEntry> Children { get; }

    public IEnumerable<PakFileEntry> GetFiles(bool recursive = false) {
        foreach (var file in GetChildren<PakFileEntry>()) yield return file;
        if (!recursive) yield break;
        foreach (var directory in GetChildren<PakDirectoryEntry>()) {
            foreach (var file in ((IPakEnumerable) directory).GetFiles(true)) yield return file;
        }
    }

    public IEnumerable<T> GetChildren<T>() where T : PakEntry {
        foreach (var child in Children) {
            if (child is T childT) yield return childT;
        }
    }
}