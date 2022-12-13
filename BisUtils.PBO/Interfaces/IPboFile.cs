using BisUtils.Core.Serialization;
using BisUtils.PBO.Builders;
using BisUtils.PBO.Entries;

namespace BisUtils.PBO.Interfaces; 

public interface IPboFile : IBisBinarizable<PboDebinarizationOptions, PboBinarizationOptions>, IDisposable {
    byte[] GetEntryData(PboDataEntry dataEntry, bool decompress = true);
    void OverwriteEntryData(PboDataEntry dataEntry, byte[] data, bool compressed = false, bool syncToStream = true);

    void AddEntry(PboDataEntryDto dataEntryDto, bool syncStream = false);
    void DeleteEntry(PboDataEntry dataEntry, bool syncStream = false);
    void RenameEntry(PboDataEntry dataEntry, string newName, bool syncStream = false);

    IEnumerable<PboEntry> GetPboEntries();
    IEnumerable<PboDataEntry>? GetDataEntries();
    IEnumerable<PboDataEntryDto>? GetDTOEntries();

    IEnumerable<PboVersionEntry>? GetVersionEntries();
    PboVersionEntry? GetVersionEntry();
}