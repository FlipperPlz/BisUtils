namespace BisUtils.RvBank.Extensions;

using Core.IO;

public static class RvBankReaderExtensions
{



    internal static ulong ReadULong32(this BisBinaryReader reader) => reader.ReadUInt32();
}
