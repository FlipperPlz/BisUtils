namespace BisUtils.Bank.Utils;

public enum PboEntryMime
{
    Decompressed = 0x00000000,
    Compressed = 0x43707273,
    Encrypted = 0x456e6372,
    Version = 0x56657273
}
