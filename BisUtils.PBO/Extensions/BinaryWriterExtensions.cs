using System.Security.Cryptography;

namespace BisUtils.PBO.Extensions; 

public static class BinaryWriterExtensions {
    internal static void WritePboChecksum(this BinaryWriter writer) {
        var oldPos = writer.BaseStream.Position;

        writer.BaseStream.Seek(0, SeekOrigin.Begin);
#pragma warning disable 
        var hash = new SHA1Managed().ComputeHash(writer.BaseStream);
#pragma warning restore 

        writer.BaseStream.Seek(oldPos, SeekOrigin.Begin);
        writer.Write((byte) 0x0);
        writer.Write(hash);
    }
}