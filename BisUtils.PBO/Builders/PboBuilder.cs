namespace BisUtils.PBO.Builders; 

public class PboBuilder {
    private readonly PboFile _pboFile;

    public PboBuilder(PboFile pboFile) {
        _pboFile = pboFile;
        throw new NotImplementedException();
    }

    public PboBuilder(string pboLocation, string? prefix = null) {
        prefix ??= Path.GetFileNameWithoutExtension(pboLocation);
        _pboFile = new PboFile(File.Create(pboLocation), PboFileOption.Create);
        _pboFile.GetVersionEntry()!.AddMetadataProperty("prefix", prefix, true);
    }

    public PboBuilder WithEntry(PboDataEntryDto dtoEntry) {
        _pboFile.AddEntry(dtoEntry, true);
        return this;
    } 

    public PboFile GetPboFile() => _pboFile;
}