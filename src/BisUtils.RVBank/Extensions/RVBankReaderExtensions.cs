namespace BisUtils.RVBank.Extensions;

using Core.IO;

public static class RVBankReaderExtensions
{



    internal static ulong ReadULong32(this BisBinaryReader reader) => reader.ReadUInt32();
}
