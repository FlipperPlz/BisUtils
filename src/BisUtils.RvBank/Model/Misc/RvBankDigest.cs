// ReSharper disable UnusedAutoPropertyAccessor.Local
namespace BisUtils.RvBank.Model.Misc;

using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Core.IO;
using Options;

public interface IRvBankDigest
{
    public int SectorA { get; }
    public int SectorB { get; }
    public int SectorC { get; }
    public int SectorD { get; }
    public int SectorE { get; }
}

public struct RvBankDigest : IRvBankDigest
{
    public int SectorA { get; private set; }
    public int SectorB { get; private set; }
    public int SectorC { get; private set; }
    public int SectorD { get; private set; }
    public int SectorE { get; private set; }


    public RvBankDigest(BisBinaryReader reader) => Read(reader);

    public RvBankDigest(byte[] c)
    {
        if (c.Length < 20)
        {
            throw new ArgumentException("`c` must be at least 20 bytes.", nameof(c));
        }

        SectorA = BitConverter.ToInt32(c, 0);
        SectorB = BitConverter.ToInt32(c, 4);
        SectorC = BitConverter.ToInt32(c, 8);
        SectorD = BitConverter.ToInt32(c, 12);
        SectorE = BitConverter.ToInt32(c, 16);
    }

    public void Read(BisBinaryReader reader)
    {
        SectorA = reader.ReadInt32();
        SectorB = reader.ReadInt32();
        SectorC = reader.ReadInt32();
        SectorD = reader.ReadInt32();
        SectorE = reader.ReadInt32();
    }

    public byte[] ToByteArray()
    {
        var byteArray = new byte[20];

        WriteBytes(SectorA, 0);
        WriteBytes(SectorB, 4);
        WriteBytes(SectorC, 8);
        WriteBytes(SectorD, 12);
        WriteBytes(SectorE, 16);

        return byteArray;

        void WriteBytes(int value, int start)
        {
            byteArray[start] = (byte)value;
            byteArray[start + 1] = (byte)(value >> 8);
            byteArray[start + 2] = (byte)(value >> 16);
            byteArray[start + 3] = (byte)(value >> 24);
        }
    }


#pragma warning disable SYSLIB0021
#pragma warning disable CA5350
    public static RvBankDigest CalculateStreamDigest(Stream stream, bool resetPosition = false)
    {
        var oldPosition = stream.Position;
        stream.Seek(0, SeekOrigin.Begin);


        using var alg = new SHA1Managed();
        var digest = new RvBankDigest(alg.ComputeHash(stream));

        if (resetPosition)
        {
            stream.Seek(oldPosition, SeekOrigin.Begin);
        }

        return digest;
    }
#pragma warning restore CA5350
#pragma warning restore SYSLIB0021

    public void Write(BisBinaryWriter writer)
    {
        writer.Write(SectorA);
        writer.Write(SectorB);
        writer.Write(SectorC);
        writer.Write(SectorD);
        writer.Write(SectorE);
    }

    public static bool operator ==(RvBankDigest obj1, RvBankDigest obj2) => obj1.Equals(obj2);
    public static bool operator !=(RvBankDigest obj1, RvBankDigest obj2) => !(obj1 == obj2);
    public bool Equals(RvBankDigest other) => SectorA.Equals(other.SectorA)
        && SectorB.Equals(other.SectorB)
        && SectorC.Equals(other.SectorC)
        && SectorD.Equals(other.SectorD)
        && SectorE.Equals(other.SectorE);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        return obj is RvBankDigest other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = SectorA.GetHashCode();
            hashCode = (hashCode * 397) ^ SectorB.GetHashCode();
            hashCode = (hashCode * 397) ^ SectorC.GetHashCode();
            hashCode = (hashCode * 397) ^ SectorD.GetHashCode();
            hashCode = (hashCode * 397) ^ SectorE.GetHashCode();

            return hashCode;
        }
    }

}


