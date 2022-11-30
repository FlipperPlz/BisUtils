using System.Security.Cryptography;

namespace BisUtils.PBO.Extensions; 

public static class BinaryReaderExtensions {
    public static bool VerifyPboChecksum(this BinaryReader reader) {
        var startPos = reader.BaseStream.Position;
        
        reader.BaseStream.Seek(0, SeekOrigin.Begin);
#pragma warning disable 
        var hash = new SHA1Managed().ComputeHash(new MemoryStream(reader.ReadBytes((int) startPos)));
#pragma warning restore 
        
        reader.ReadByte();

        return hash.SequenceEqual(reader.ReadBytes(hash.Length));

    }
    
    
}