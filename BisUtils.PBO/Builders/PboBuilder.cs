using System.Text;

namespace BisUtils.PBO.Builders; 

public class PboBuilder {
    private readonly PboFile _pboFile;

    public PboBuilder(PboFile pboFile) {
        _pboFile = pboFile;
        throw new NotImplementedException();
    }

    public PboBuilder(string pboLocation, string? prefix = null) {
        prefix ??= Path.GetFileNameWithoutExtension(pboLocation);
        _pboFile = new PboFile(pboLocation, PboFileOption.Create);
        _pboFile.GetVersionEntry()!.AddMetadataProperty("prefix", prefix, true);
    }

    public PboBuilder WithEntry(PboDataEntryDto dtoEntry) {
        _pboFile.AddEntry(dtoEntry, true);
        return this;
    } 
    
    public PboBuilder WithEntry(string entryName, string entryData, ulong? timestamp = null, bool compress = false) {
        _pboFile.AddEntry(new PboDataEntryDto(_pboFile, 
            new MemoryStream(Encoding.UTF8.GetBytes(entryData)), timestamp, compress), true);
        return this;
    } 

    public PboFile GetPboFile() => _pboFile;
}